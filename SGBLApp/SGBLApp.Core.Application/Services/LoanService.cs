using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using SGBLApp.Core.Application.DTOs.Loans;
using Microsoft.AspNetCore.Identity;
using SGBLApp.Core.Application.Utilities;

namespace SGBLApp.Core.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly int _maxLoansPerUser;
        private readonly int _loanDurationDays;
        private readonly IReservationRepository _reservationRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanService(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IMapper mapper,
            IConfiguration configuration,
            IReservationRepository reservationRepository,
            INotificationService notificationService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _configuration = configuration;
            _reservationRepository = reservationRepository;
            _notificationService = notificationService;
            _emailService = emailService;
            _userManager = userManager;

            // Obtener configuración - valores por defecto si no están definidos
            _maxLoansPerUser = _configuration.GetValue<int>("LoanSettings:MaxLoansPerUser", 3);
            _loanDurationDays = _configuration.GetValue<int>("LoanSettings:DefaultLoanDurationDays", 21);
        }

        public int MaxLoansPerUser => _maxLoansPerUser;
        public int DefaultLoanDurationDays => _loanDurationDays;

        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = _loanRepository.GetAll();
            return loans.Select(MapToDto).ToList();
        }

        public async Task<LoanDto> GetLoanByIdAsync(int id)
        {
            var loan = _loanRepository.GetById(id);
            return loan != null ? MapToDto(loan) : null;
        }

        public async Task<IEnumerable<LoanDto>> GetLoansByUserAsync(string userId)
        {
            var loans = await _loanRepository.GetLoansByUserAsync(userId);
            return loans.Select(MapToDto).ToList();
        }

        public async Task<(bool canRequest, string message)> CanRequestLoanAsync(string userId)
        {
            var activeLoansCount = await _loanRepository.GetActiveLoansCountByUserAsync(userId);
            if (activeLoansCount >= MaxLoansPerUser)
                return (false, $"Límite de préstamos alcanzado (máximo {MaxLoansPerUser}).");

            var hasOverdueLoans = await _loanRepository.HasOverdueLoansAsync(userId);
            if (hasOverdueLoans)
                return (false, "Tienes préstamos vencidos. No puedes solicitar más.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user?.LoanPenaltyUntil > DateTime.UtcNow)
                return (false, $"Penalizado por préstamos vencidos hasta {user.LoanPenaltyUntil:d}.");

            return (true, "Puedes solicitar el préstamo.");
        }

        public async Task<(bool success, string message)> RequestLoanAsync(LoanRequestDto loanRequest)
        {
            var (canRequest, message) = await CanRequestLoanAsync(loanRequest.UserId);
            if (!canRequest)
                return (false, message);

            var book = _bookRepository.GetById(loanRequest.BookId);
            if (book?.AvailableCopies <= 0)
                return (false, "No hay copias disponibles de este libro.");

            if (book.Status != BookStatus.Disponible)
                return (false, "El libro no está disponible para préstamo.");

            if (loanRequest.DueDate == DateTime.MinValue)
                loanRequest.DueDate = DateTime.Today.AddDays(DefaultLoanDurationDays);

            if (await _loanRepository.HasActiveLoanForBookAsync(loanRequest.UserId, loanRequest.BookId))
                return (false, "Ya tienes este libro prestado.");

            var loan = new Loan
            {
                BookId = loanRequest.BookId,
                UserId = loanRequest.UserId,
                LoanDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(DefaultLoanDurationDays),
                Status = LoanStatus.Pendiente
            };

            _loanRepository.Add(loan);

            return (true, "Solicitud de préstamo registrada correctamente. Espera la aprobación.");
        }

        public async Task<IEnumerable<LoanDto>> GetPendingLoansAsync()
        {
            var loans = _loanRepository.GetAll().Where(l => l.Status == LoanStatus.Pendiente);
            return loans.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _loanRepository.GetOverdueLoansAsync();
            return loans.Select(MapToDto).ToList();
        }

        public async Task<(bool success, string message)> ApproveLoanAsync(int loanId)
        {
            var loan = await _loanRepository.GetLoanDetailsAsync(loanId);
            if (loan == null)
                return (false, "El préstamo no existe.");

            if (loan.Status != LoanStatus.Pendiente)
                return (false, "Solo se pueden aprobar préstamos pendientes.");

            var book = _bookRepository.GetById(loan.BookId);
            if (book == null)
                return (false, "El libro asociado no existe.");

            // Verificar límite de préstamos del usuario
            var activeLoansCount = await _loanRepository.GetActiveLoansCountByUserAsync(loan.UserId);
            if (activeLoansCount >= MaxLoansPerUser)
                return (false, "El usuario ha alcanzado el límite máximo de préstamos.");

            // Verificar préstamos vencidos
            if (await _loanRepository.HasOverdueLoansAsync(loan.UserId))
                return (false, "El usuario tiene préstamos vencidos.");

            // Verificar disponibilidad real del libro
            if (book.AvailableCopies <= 0)
                return (false, "No hay copias disponibles de este libro.");

            // Actualizar préstamo
            loan.Status = LoanStatus.Aprobado;
            _loanRepository.Update(loan);

            // Actualizar libro
            book.AvailableCopies--;
            book.Status = book.AvailableCopies > 0 ? BookStatus.Disponible : BookStatus.NoDisponible;
            _bookRepository.Update(book);

            return (true, "Préstamo aprobado correctamente.");
        }

        public async Task<(bool success, string message)> RejectLoanAsync(int loanId, string reason)
        {
            var loan = _loanRepository.GetById(loanId);
            if (loan == null)
                return (false, "El préstamo no existe.");

            if (loan.Status != LoanStatus.Pendiente)
                return (false, "Solo se pueden rechazar préstamos pendientes.");

            // Rechazar el préstamo
            loan.Status = LoanStatus.Rechazado;
            _loanRepository.Update(loan);

            return (true, "Préstamo rechazado correctamente.");
        }

        public async Task<Result> ReturnBookAsync(LoanReturnDto returnDto)
        {
            var loan = await _loanRepository.GetLoanDetailsAsync(returnDto.LoanId);
            if (loan == null || loan.Status != LoanStatus.Aprobado)
                return Result.Failure("Préstamo no válido o ya devuelto");

            var book = _bookRepository.GetById(loan.BookId);
            if (book == null)
                return Result.Failure("Libro no encontrado");

            if (loan.DueDate < DateTime.UtcNow)
            {
                var user = await _userManager.FindByIdAsync(loan.UserId);
                user.LoanPenaltyUntil = DateTime.UtcNow.AddMonths(3);
                await _userManager.UpdateAsync(user);
            }

            // Registrar devolución
            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = LoanStatus.Devuelto;
            _loanRepository.Update(loan);

            // Actualizar libro
            book.AvailableCopies++;
            book.Status = book.AvailableCopies > 0 ? BookStatus.Disponible : BookStatus.NoDisponible;
            _bookRepository.Update(book);

            // Procesar reservas
            var nextReservation = await _reservationRepository.GetNextReservationForBookAsync(book.BookId);
            if (nextReservation != null)
            {
                // Actualizar estado de reserva
                nextReservation.Status = ReservationStatus.Disponible;
                nextReservation.ExpirationDate = DateTime.UtcNow.AddHours(48);
                await _reservationRepository.UpdateAsync(nextReservation);
                await _reservationRepository.SaveChangesAsync();

                // Crear notificación
                await _notificationService.SendReservationAvailableNotificationAsync(nextReservation.ReservationId);
            }

            _loanRepository.SaveChanges();
            return Result.Success();
        }

        // Método auxiliar para mapear Loan a LoanDto
        private LoanDto MapToDto(Loan loan)
        {
            return new LoanDto
            {
                LoanId = loan.LoanId,
                BookTitle = loan.Book?.Title ?? "Libro no disponible",
                UserName = loan.User?.Name ?? "Usuario no disponible",
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
            };
        }
    }
}

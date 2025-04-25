using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SGBLApp.Core.Application.DTOs.Loans;
using SGBLApp.Core.Application.DTOs.Reservation;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Utilities;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;


namespace SGBLApp.Core.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly int _maxReservationsPerUser;

        public ReservationService(
            IReservationRepository reservationRepository,
            IBookRepository bookRepository,
            ILoanRepository loanRepository,
            INotificationService notificationService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _reservationRepository = reservationRepository;
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _configuration = configuration;
            _maxReservationsPerUser = _configuration.GetValue<int>("ReservationSettings:MaxReservationsPerUser", 3);
        }

        public async Task<Result<ReservationDto>> CreateReservationAsync(ReservationRequestDto request)
        {
            // Validación temprana
            if (request.BookId <= 0 || string.IsNullOrEmpty(request.UserId))
                return Result<ReservationDto>.Failure("Datos inválidos");

            // Lógica de negocio centralizada
            var book = _bookRepository.GetById(request.BookId);
            if (book == null)
                return Result<ReservationDto>.Failure("Libro no encontrado");

            bool hasActiveReservation = await _reservationRepository.HasActiveReservationForBookAsync(request.UserId, request.BookId);
            if (hasActiveReservation)
                return Result<ReservationDto>.Failure("Ya tienes una reserva activa para este libro");

            // Validación de límite de reservas
            var activeReservations = await _reservationRepository.GetActiveReservationCountForUserAsync(request.UserId);
            if (activeReservations >= _maxReservationsPerUser)
                return Result<ReservationDto>.Failure($"Límite de reservas alcanzado (máximo {_maxReservationsPerUser})");

            // Crear entidad
            var reservation = new Reservation
            {
                BookId = request.BookId,
                UserId = request.UserId,
                Status = ReservationStatus.Pendiente,
                ReservationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            return Result<ReservationDto>.Success(
                _mapper.Map<ReservationDto>(reservation)
            );
        }

        private async Task ProcessAvailableReservation(Reservation reservation)
        {
            reservation.Status = ReservationStatus.Disponible;
            reservation.ExpirationDate = DateTime.UtcNow.AddDays(2); // 48 horas para reclamar
            await _reservationRepository.UpdateAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            await _notificationService.SendReservationAvailableNotificationAsync(reservation.ReservationId);
        }


        public async Task<Result<ReservationDto>> GetReservationDetailsAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                return Result<ReservationDto>.Failure("Reserva no encontrada");

            var dto = _mapper.Map<ReservationDto>(reservation);

            // Añadir lógica adicional si es necesario
            dto.BookTitle = reservation.Book?.Title ?? "Libro no disponible";
            dto.UserEmail = reservation.User?.Email ?? "Usuario no disponible";

            return Result<ReservationDto>.Success(dto);
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null) return;

            reservation.Status = ReservationStatus.Cancelada;
            await _reservationRepository.UpdateAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            // Notificar siguiente reserva si existe
            var nextReservation = await _reservationRepository.GetNextReservationForBookAsync(reservation.BookId);
            if (nextReservation != null)
            {
                await ProcessAvailableReservation(nextReservation);
            }
        }

        public async Task<Result<LoanDto>> FulfillReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation?.Status != ReservationStatus.Disponible)
                return Result<LoanDto>.Failure("Reserva no válida o no disponible");

            var book = _bookRepository.GetById(reservation.BookId);
            if (book?.AvailableCopies <= 0)
                return Result<LoanDto>.Failure("El libro ya no está disponible");

            // Crear préstamo
            var loan = new Loan
            {
                BookId = reservation.BookId,
                UserId = reservation.UserId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(21),
                Status = LoanStatus.Aprobado
            };

            _loanRepository.Add(loan);

            // Actualizar reserva y libro
            reservation.Status = ReservationStatus.Completada;
            book.AvailableCopies--;

            await _reservationRepository.UpdateAsync(reservation);
            _bookRepository.Update(book);
            await _reservationRepository.SaveChangesAsync();

            // Procesar siguiente reserva si existe
            var nextReservation = await _reservationRepository.GetNextReservationForBookAsync(reservation.BookId);
            if (nextReservation != null)
            {
                await ProcessAvailableReservation(nextReservation);
            }

            return Result<LoanDto>.Success(_mapper.Map<LoanDto>(loan));
        }

        public async Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId)
        {
            var reservations = await _reservationRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ReservationDto>>(reservations);
        }

        public async Task CheckAndExpireReservationsAsync()
        {
            var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync();
            foreach (var reservation in expiredReservations)
            {
                if (reservation.ExpirationDate < DateTime.UtcNow)
                {
                    reservation.Status = ReservationStatus.Expirada;
                    await _reservationRepository.UpdateAsync(reservation);

                    // Notificar siguiente reserva
                    var nextReservation = await _reservationRepository.GetNextReservationForBookAsync(reservation.BookId);
                    if (nextReservation != null)
                    {
                        await ProcessAvailableReservation(nextReservation);
                    }
                }
            }
            await _reservationRepository.SaveChangesAsync();
        }
    }
}


using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SGBLApp.Core.Application.DTOs.Notification;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Utilities;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Core.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            IReservationRepository reservationRepository,
            ILoanRepository loanRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _reservationRepository = reservationRepository;
            _loanRepository = loanRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task SendReservationAvailableNotificationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation?.Status != ReservationStatus.Disponible) return;

            // Crear notificación en sistema
            var notification = new Notification
            {
                UserId = reservation.UserId,
                ReservationId = reservationId,
                Title = "Libro disponible para retiro",
                Message = $"El libro '{reservation.Book.Title}' está listo para ser recogido",
                CreatedAt = DateTime.UtcNow
            };

            _notificationRepository.Add(notification);
            _notificationRepository.SaveChanges();

            // Enviar email
            await _emailService.SendEmailAsync(
                reservation.User.UserName,
                "Libro disponible en la biblioteca",
                $"<p>Estimado/a {reservation.User.Name},</p>\r\n" +
                "<p>Nos complace informarte que el libro que reservaste está disponible para retiro:</p>\r\n" +
                $"<p><strong>Título del libro:</strong> {reservation.Book.Title}</p>\r\n" +
                "<p>Tienes un plazo de 48 horas para reclamarlo antes de que la reserva expire.</p>\r\n" +
                "<p>Atentamente,</p>\r\n" +
                "<p>Equipo de la Biblioteca</p>\r\n");
        }

        public async Task SendDueDateRemindersAsync()
        {
            var dueSoonLoans = await _loanRepository.GetLoansDueSoonAsync(DateTime.UtcNow.AddDays(1));
            foreach (var loan in dueSoonLoans)
            {
                var notification = new Notification
                {
                    UserId = loan.UserId,
                    LoanId = loan.LoanId,
                    Title = "Recordatorio de vencimiento",
                    Message = $"El préstamo del libro '{loan.Book.Title}' vence pronto",
                    CreatedAt = DateTime.UtcNow
                };

                _notificationRepository.Add(notification);

                await _emailService.SendEmailAsync(
                    loan.User.Email,
                    "Recordatorio de vencimiento de préstamo",
                    $"El libro '{loan.Book.Title}' debe ser devuelto antes de {loan.DueDate:dd/MM/yyyy}");
            }
            _notificationRepository.SaveChanges();
        }

        public async Task<Result> MarkNotificationAsReadAsync(int notificationId)
        {
            try
            {
                var notification = _notificationRepository.GetById(notificationId);
                if (notification == null)
                    return Result.Failure("Notificación no encontrada");

                await _notificationRepository.MarkAsReadAsync(notificationId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error al marcar como leída: {ex.Message}");
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task ProcessReservationNotificationsAsync(int bookId)
        {
            var nextReservation = await _reservationRepository.GetNextReservationForBookAsync(bookId);
            if (nextReservation != null)
            {
                await SendReservationAvailableNotificationAsync(nextReservation.ReservationId);
            }
        }

        public async Task<Result<bool>> DeleteNotificationAsync(int notificationId)
        {
            var notification = _notificationRepository.GetById(notificationId);
            if (notification == null)
                return Result<bool>.Failure("Notificación no encontrada");

            _notificationRepository.Delete(notification);
            _notificationRepository.SaveChanges();

            return Result<bool>.Success(true);
        }

        public async Task MarkAllNotificationsAsReadAsync(string userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            _notificationRepository.SaveChanges();
        }
    }
}

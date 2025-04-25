using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs.Notification;
using SGBLApp.Core.Application.Utilities;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendReservationAvailableNotificationAsync(int reservationId);
        Task SendDueDateRemindersAsync();
        Task<Result> MarkNotificationAsReadAsync(int notificationId);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task ProcessReservationNotificationsAsync(int bookId);
        Task<Result<bool>> DeleteNotificationAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(string userId);
    }
}

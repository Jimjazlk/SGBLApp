using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Domain.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task CreateReservationAvailableNotification(int reservationId);
        Task<List<Notification>> GetByLoanIdAsync(int loanId);

    }
}

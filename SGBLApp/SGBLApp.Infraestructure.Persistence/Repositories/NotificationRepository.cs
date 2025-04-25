using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;


namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationContext _context;

        public NotificationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Notification> GetAll()
        {
            return _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Reservation)
                .Include(n => n.Loan)
                .AsNoTracking()
                .ToList();
        }

        public Notification? GetById(int id)
        {
            return _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Reservation)
                .Include(n => n.Loan)
                .FirstOrDefault(n => n.Id == id);
        }

        public void Add(Notification entity)
        {
            _context.Notifications.Add(entity);
        }

        public void Update(Notification entity)
        {
            _context.Notifications.Update(entity);
        }

        public void Delete(Notification entity)
        {
            _context.Notifications.Remove(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Reservation)
                .Include(n => n.Loan)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateReservationAvailableNotification(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation != null)
            {
                var notification = new Notification
                {
                    UserId = reservation.UserId,
                    ReservationId = reservation.ReservationId,
                    Title = "¡Libro disponible para préstamo!",
                    Message = $"Estimado/a {reservation.User.UserName},\n\n" +
                      $"Nos complace informarle que el libro \"{reservation.Book.Title}\" que reservó se encuentra ahora disponible para préstamo. " +
                      "Le invitamos a ingresar a su cuenta en línea o a visitar nuestra biblioteca para gestionar el retiro del mismo.\n\n" +
                      "Atentamente,\nSGBLApp team",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                Add(notification);
                SaveChanges();
            }
        }

        public async Task<List<Notification>> GetByLoanIdAsync(int loanId)
        {
            return await _context.Notifications
                .Include(n => n.Loan)
                .Where(n => n.LoanId == loanId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}

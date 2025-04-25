using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationContext _context;

        public ReservationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Reservation> GetAll()
        {
            return _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .AsNoTracking()
                .ToList();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task AddAsync(Reservation entity)
        {
            await _context.Reservations.AddAsync(entity);
        }
        public async Task UpdateAsync(Reservation entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Delete(Reservation entity)
        {
            _context.Reservations.Remove(entity);
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status == ReservationStatus.Pendiente || r.Status == ReservationStatus.Disponible)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.Status == ReservationStatus.Expirada)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByUserAndBookAsync(string userId, int bookId)
        {
            return await _context.Reservations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId &&
                                       (r.Status == ReservationStatus.Pendiente || r.Status == ReservationStatus.Disponible));
        }

        public async Task<int> GetActiveReservationCountForUserAsync(string userId)
        {
            return await _context.Reservations
                .CountAsync(r => r.UserId == userId &&
                                (r.Status == ReservationStatus.Pendiente || r.Status == ReservationStatus.Disponible));
        }

        public async Task<bool> HasActiveReservationForBookAsync(string userId, int bookId)
        {
            return await _context.Reservations
                .AnyAsync(r => r.UserId == userId && r.BookId == bookId &&
                              (r.Status == ReservationStatus.Pendiente || r.Status == ReservationStatus.Disponible));
        }

        public async Task<Reservation?> GetNextReservationForBookAsync(int bookId)
        {
            return await _context.Reservations
                .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Pendiente)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsForBookAsync(int bookId)
        {
            return await _context.Reservations
                .Where(r => r.BookId == bookId &&
                          (r.Status == ReservationStatus.Pendiente ||
                           r.Status == ReservationStatus.Disponible))
                .ToListAsync();
        }
    }
}

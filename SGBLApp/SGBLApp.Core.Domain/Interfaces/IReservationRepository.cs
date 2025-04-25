using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;


namespace SGBLApp.Core.Domain.Interfaces
{
    public interface IReservationRepository
    {
        IEnumerable<Reservation> GetAll();
        Task<Reservation?> GetByIdAsync(int id);
        Task AddAsync(Reservation entity);
        Task UpdateAsync(Reservation entity);
        void Delete(Reservation entity);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
        Task<Reservation?> GetByUserAndBookAsync(string userId, int bookId);
        Task<int> GetActiveReservationCountForUserAsync(string userId);
        Task<bool> HasActiveReservationForBookAsync(string userId, int bookId);
        Task<Reservation> GetNextReservationForBookAsync(int bookId);
        Task<IEnumerable<Reservation>> GetActiveReservationsForBookAsync(int bookId);

    }

}

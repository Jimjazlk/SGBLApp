using SGBLApp.Core.Application.DTOs.Loans;
using SGBLApp.Core.Application.DTOs.Reservation;
using SGBLApp.Core.Application.Utilities;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Result<ReservationDto>> CreateReservationAsync(ReservationRequestDto request);
        Task<Result<ReservationDto>> GetReservationDetailsAsync(int reservationId);
        Task CancelReservationAsync(int reservationId);
        Task<Result<LoanDto>> FulfillReservationAsync(int reservationId);
        Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string userId);
        Task CheckAndExpireReservationsAsync();
    }
}

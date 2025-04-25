using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Core.Application.DTOs.Reservation
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public string BookTitle { get; set; }
        public string UserEmail { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public ReservationStatus Status { get; set; }
        
    }
}

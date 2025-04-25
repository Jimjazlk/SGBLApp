using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Core.Domain.Entities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pendiente;

        public Book? Book { get; set; }
        public ApplicationUser? User { get; set; }
    }
}

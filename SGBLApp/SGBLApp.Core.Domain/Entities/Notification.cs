
namespace SGBLApp.Core.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
        public int? LoanId { get; set; }
        public int? ReservationId { get; set; }

        public ApplicationUser User { get; set; }
        public Reservation Reservation { get; set; }
        public Loan Loan { get; set; }
    }
}

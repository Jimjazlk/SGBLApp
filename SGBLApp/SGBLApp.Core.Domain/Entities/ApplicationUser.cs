using Microsoft.AspNetCore.Identity;

namespace SGBLApp.Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int LibraryId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LoanPenaltyUntil { get; set; }


        // RELACIONES
        public Library Library { get; set; } // 1 a 1
        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
    }
}

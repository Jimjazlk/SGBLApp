
using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Core.Domain.Entities
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public Book? Book { get; set; }
        public ApplicationUser? User { get; set; }
    }
}

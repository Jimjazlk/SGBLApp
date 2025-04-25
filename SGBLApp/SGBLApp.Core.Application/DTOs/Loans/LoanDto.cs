using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Core.Application.DTOs.Loans
{
    public class LoanDto
    {
        public int LoanId { get; set; }
        public required string BookTitle { get; set; }
        public required string UserName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public bool IsOverdue => Status == LoanStatus.Aprobado && DueDate < DateTime.Now;
    }
}

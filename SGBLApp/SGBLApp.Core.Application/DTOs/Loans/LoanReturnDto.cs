using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Core.Application.DTOs.Loans
{
    public class LoanReturnDto
    {
        public int LoanId { get; set; }

        [Display(Name = "Título del libro")]
        public string BookTitle { get; set; }

        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Display(Name = "Fecha de préstamo")]
        public DateTime LoanDate { get; set; }

        [Display(Name = "Fecha límite")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Fecha de devolución")]
        public DateTime ReturnDate { get; set; } = DateTime.Today;

        public bool IsOverdue => DateTime.Today > DueDate;
    }
}

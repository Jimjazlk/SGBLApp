
using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Core.Application.DTOs.Loans
{
    public class LoanRequestDto
    {
        public int BookId { get; set; }

        [Display(Name = "Título del libro")]
        public required string BookTitle { get; set; }

        public required string UserId { get; set; }

        [Display(Name = "Usuario")]
        public required string UserName { get; set; }

        [Display(Name = "Fecha de préstamo")]
        public DateTime LoanDate { get; set; } = DateTime.Today;

        [Display(Name = "Fecha de devolución")]
        public DateTime DueDate { get; set; }
    }
}

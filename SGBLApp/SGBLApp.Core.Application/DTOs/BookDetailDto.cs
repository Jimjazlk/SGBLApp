
namespace SGBLApp.Core.Application.DTOs
{
    public class BookDetailDto : BookDto
    {
        public int LoanCount { get; set; }
        public int ReservationCount { get; set; }
    }
}

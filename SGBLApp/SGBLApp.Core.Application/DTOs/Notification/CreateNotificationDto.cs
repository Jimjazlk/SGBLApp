using System.ComponentModel.DataAnnotations;


namespace SGBLApp.Core.Application.DTOs.Notification
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public int? LoanId { get; set; }
        public int? ReservationId { get; set; }
    }
}

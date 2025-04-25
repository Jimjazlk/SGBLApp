namespace SGBLApp.Core.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int? RelatedLoanId { get; set; }
        public int? RelatedReservationId { get; set; }

        // Para visualización
        public string BookTitle { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

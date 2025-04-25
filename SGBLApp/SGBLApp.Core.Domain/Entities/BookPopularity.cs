
using System.ComponentModel.DataAnnotations.Schema;

namespace SGBLApp.Core.Domain.Entities
{
    public class BookPopularity
    {
        public int BookId { get; set; }
        public int LoanCount { get; set; }
        public int ReservationCount { get; set; }
        public int ClickCount { get; set; }
        public int RecommendationFeedbackPositive { get; set; }
        public int RecommendationFeedbackNegative { get; set; }

        [NotMapped]
        public int TotalPopularityScore => LoanCount + ReservationCount + (ClickCount / 2);
        public Book Book { get; set; }
    }
}

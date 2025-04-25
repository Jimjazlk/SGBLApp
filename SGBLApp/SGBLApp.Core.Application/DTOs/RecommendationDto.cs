
namespace SGBLApp.Core.Application.DTOs
{
    public class RecommendationDto
    {
        public IEnumerable<BookDto> PersonalizedRecommendations { get; set; }
        public IEnumerable<BookDto> PopularRecommendations { get; set; }
        public IEnumerable<BookDto> CollaborativeRecommendations { get; set; }
        public List<BookDto> LikedBooks { get; set; }
        public List<BookDto> DislikedBooks { get; set; }

    }
}

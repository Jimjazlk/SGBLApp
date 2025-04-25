
namespace SGBLApp.Core.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int OverdueLoans { get; set; }
        public List<BookDto> MostPopularBooks { get; set; }  // Basado en PopularityScore  
        public Dictionary<string, int> GenreDistribution { get; set; }  // Ej: {"Ficción": 15, "Tecnología": 8}  
    }
}

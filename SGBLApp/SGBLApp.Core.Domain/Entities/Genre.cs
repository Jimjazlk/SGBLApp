
namespace SGBLApp.Core.Domain.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public ICollection<Book?> PrimaryBooks { get; set; } = null!;
        public ICollection<Book?> SecondaryBooks { get; set; } = null!;
    }
}

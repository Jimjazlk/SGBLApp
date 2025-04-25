
namespace SGBLApp.Core.Domain.Entities
{
    public class Library
    {
        public int LibraryId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public ICollection<Book?> Books { get; set; } = null!;
        public ICollection<ApplicationUser?> Users { get; set; } = null!;
    }
}

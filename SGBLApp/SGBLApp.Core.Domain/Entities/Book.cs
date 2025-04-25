using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Core.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public string ImageUrl { get; set; }
        public BookStatus Status { get; set; }
        public int? PopularityScore => BookPopularity?.LoanCount * 2 +
                             BookPopularity?.ReservationCount +
                             (BookPopularity?.ClickCount ?? 0) / 2;
        public int LibraryId { get; set; }
        public int AuthorId { get; set; }
        public int Copies { get; set; }
        public int AvailableCopies { get; set; }

        // GENRE
        public int PrimaryGenreId { get; set; }
        public Genre PrimaryGenre { get; set; } = null!;

        public int? SecondaryGenreId { get; set; }
        public Genre? SecondaryGenre { get; set; }


        #region NavProperties
        public Library Library { get; set; } = null!;
        public Author Author { get; set; } = null!;
        public BookPopularity BookPopularity { get; set; }
        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        #endregion
    }
}

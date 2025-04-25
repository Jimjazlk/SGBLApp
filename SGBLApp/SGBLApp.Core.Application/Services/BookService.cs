using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Core.Application.Services
{
    public class BookService : IBaseService<BookDto>, IBookService 
    {
        private readonly IBookRepository _bookRepository;
        private readonly IReservationRepository _reservationRepository;


        public BookService(IBookRepository bookRepository, IReservationRepository reservationRepository)
        {
            _bookRepository = bookRepository;
            _reservationRepository = reservationRepository;
        }

        #region IBaseService Implementation

        public IEnumerable<BookDto> GetAll()
        {
            return _bookRepository.GetAll().Select(b => new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                ImageUrl = b.ImageUrl,
                Status = b.Status,
                PopularityScore = (b.BookPopularity?.LoanCount ?? 0) * 2 +
                 (b.BookPopularity?.ReservationCount ?? 0) +
                 (b.BookPopularity?.ClickCount ?? 0) / 2,
                LibraryId = b.LibraryId,
                LibraryName = b.Library?.Name,
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.Name,
                PrimaryGenreId = b.PrimaryGenreId,
                PrimaryGenreName = b.PrimaryGenre?.Name,
                SecondaryGenreId = b.SecondaryGenreId,
                Copies = b.Copies,
                AvailableCopies = b.AvailableCopies
            });
        }

        public BookDto? GetById(int id)
        {
            var b = _bookRepository.GetById(id);
            if (b == null)
            {
                return null;
            }

            return new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                ImageUrl = b.ImageUrl,
                Status = b.Status,
                Copies = b.Copies,
                AvailableCopies = b.AvailableCopies,
                PopularityScore = (b.BookPopularity?.LoanCount ?? 0) * 2 +
                 (b.BookPopularity?.ReservationCount ?? 0) +
                 (b.BookPopularity?.ClickCount ?? 0) / 2,
                LibraryId = b.LibraryId,
                LibraryName = b.Library?.Name,
                AuthorId = b.AuthorId,
                AuthorName = b.Author?.Name,
                PrimaryGenreId = b.PrimaryGenreId,
                PrimaryGenreName = b.PrimaryGenre?.Name,
                SecondaryGenreId = b.SecondaryGenreId,
                SecondaryGenreName = b.SecondaryGenre?.Name
            };
        }

        public void Add(BookDto dto)
        {
            var b = new Book
            {
                BookId = 0,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                ISBN = dto.ISBN,
                PublicationYear = dto.PublicationYear,
                ImageUrl = dto.ImageUrl,
                LibraryId = dto.LibraryId,
                AuthorId = dto.AuthorId,
                PrimaryGenreId = dto.PrimaryGenreId,
                SecondaryGenreId = dto.SecondaryGenreId,
                Copies = dto.Copies,
                AvailableCopies = dto.Copies,
                Status = dto.Copies > 0 ? BookStatus.Disponible : BookStatus.NoDisponible,
                BookPopularity = new BookPopularity
                {
                    LoanCount = 0,
                    ReservationCount = 0,
                    ClickCount = 0
                }
            };

            _bookRepository.Add(b);
        }

        public void Update(BookDto dto)
        {
            var existingBook = _bookRepository.GetById(dto.BookId);
            if (existingBook == null)
            {
                throw new KeyNotFoundException("El libro no existe en el sistema");
            }

            if (dto.AvailableCopies > dto.Copies)
            {
                throw new InvalidOperationException(
                    "Las copias disponibles no pueden exceder las copias totales");
            }

            if (dto.Copies < existingBook.Copies)
            {
                var diferencia = existingBook.Copies - dto.Copies;
                existingBook.AvailableCopies = Math.Max(
                    existingBook.AvailableCopies - diferencia,
                    0
                );
            }

            existingBook.Title = dto.Title;
            existingBook.Description = dto.Description ?? string.Empty;
            existingBook.ISBN = dto.ISBN;
            existingBook.PublicationYear = dto.PublicationYear;
            existingBook.ImageUrl = dto.ImageUrl;
            existingBook.LibraryId = dto.LibraryId;
            existingBook.AuthorId = dto.AuthorId;
            existingBook.PrimaryGenreId = dto.PrimaryGenreId;
            existingBook.SecondaryGenreId = dto.SecondaryGenreId;
            existingBook.Copies = dto.Copies;
            existingBook.AvailableCopies = dto.AvailableCopies;

            var activeReservations = _reservationRepository.GetActiveReservationsForBookAsync(dto.BookId).Result;
            var hasActiveReservations = activeReservations.Any();
            existingBook.Status = existingBook.AvailableCopies > 0
                ? BookStatus.Disponible
                : hasActiveReservations
                    ? BookStatus.Reservado
                    : BookStatus.NoDisponible;

            _bookRepository.Update(existingBook);
        }

        public void Delete(int id)
        {
            var b = _bookRepository.GetById(id);
            if (b != null)
            {
                _bookRepository.Delete(b);
            }
        }

        #endregion

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string title, string author, string genre)
        {
            var books = await _bookRepository.SearchBooksAsync(title, author, genre);
            return books.Select(b => new BookDto
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                ImageUrl = b.ImageUrl,
                Status = b.Status,
                PopularityScore = (b.BookPopularity?.LoanCount ?? 0) * 2 +
                                  (b.BookPopularity?.ReservationCount ?? 0) +
                                  (b.BookPopularity?.ClickCount ?? 0) / 2,
                LibraryId = b.LibraryId,
                AuthorId = b.AuthorId,
                PrimaryGenreId = b.PrimaryGenreId,
                SecondaryGenreId = b.SecondaryGenreId,
                AuthorName = b.Author?.Name,
                PrimaryGenreName = b.PrimaryGenre?.Name,
                SecondaryGenreName = b.SecondaryGenre?.Name,
                LibraryName = b.Library?.Name,

                // 👇 Agrega esto para permitir que el controlador actualice el valor
                UserLiked = null
            });
        }

    }
}

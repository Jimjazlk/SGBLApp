
using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class BookRepository : IBaseRepository<Book>, IBookRepository
    {
        private readonly ApplicationContext _context;

        public BookRepository(ApplicationContext context)
        {
            _context = context;
        }
        #region IBaseRepository Implementation
        public IEnumerable<Book> GetAll()
        {
            return _context.Books
                .Include(b => b.Library)
                .Include(b => b.Author)
                .Include(b => b.PrimaryGenre)
                .Include(b => b.SecondaryGenre)
                .Include(b => b.BookPopularity)
                .ToList();
        }

        public Book? GetById(int id)
        {
            return _context.Books
                .Include(b => b.Library)
                .Include(b => b.Author)
                .Include(b => b.PrimaryGenre)
                .Include(b => b.SecondaryGenre)
                .Include(b => b.BookPopularity)
                .FirstOrDefault(b => b.BookId == id);
        }

        public void Add(Book? entity)
        {
            _context.Books.Add(entity);
            SaveChanges();
        }

        public void Update(Book entity)
        {
            _context.Update(entity);
            SaveChanges();
        }

        public void Delete(Book? entity)
        {
            _context.Books.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string title, string author, string genre)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.PrimaryGenre)
                .Include(b => b.SecondaryGenre)
                .Include(b => b.Library)
                .Include(b => b.BookPopularity)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Name.Contains(author));

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(b => b.PrimaryGenre.Name.Contains(genre) ||
                                       (b.SecondaryGenre != null && b.SecondaryGenre.Name.Contains(genre)));

            return await query.ToListAsync();
        }
        #endregion

        public async Task<int> GetTotalBookCountAsync()
        {
            return await _context.Books.CountAsync();
        }

    }
}

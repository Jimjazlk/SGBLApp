using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class AuthorRepository : IBaseRepository<Author>, IAuthorRepository
    {
        private readonly ApplicationContext _context;

        public AuthorRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Author> GetAll() { return _context.Authors.ToList(); }

        public Author? GetById(int id)
        {
            return _context.Authors.Find(id);
        }

        public void Add(Author entity)
        {
            _context.Authors.Add(entity);
            SaveChanges();
        }

        public void Update(Author entity)
        {
            _context.Authors.Update(entity);
            SaveChanges();
        }

        public void Delete(Author entity)
        {
            _context.Authors.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges() => _context.SaveChanges();

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }
    }
}

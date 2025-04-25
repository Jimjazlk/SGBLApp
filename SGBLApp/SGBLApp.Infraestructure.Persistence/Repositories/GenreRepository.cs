using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class GenreRepository : IBaseRepository<Genre>, IGenreRepository
    {
        private readonly ApplicationContext _context;

        public GenreRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Genre> GetAll() { return _context.Genres.ToList(); }

        public Genre? GetById(int id)
        {
            return _context.Genres.Find(id);
        }

        public void Add(Genre entity)
        {
            _context.Genres.Add(entity);
            SaveChanges();
        }

        public void Update(Genre entity)
        {
            _context.Genres.Update(entity);
            SaveChanges();
        }

        public void Delete(Genre entity)
        {
            _context.Genres.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges() => _context.SaveChanges();

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }
    }
}

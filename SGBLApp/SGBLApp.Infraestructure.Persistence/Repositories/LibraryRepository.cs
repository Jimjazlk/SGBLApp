using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class LibraryRepository : ILibraryRepository, IBaseRepository<Library>
    {
        private readonly ApplicationContext _context;

        public LibraryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Library> GetAll() { return _context.Libraries.ToList(); }

        public Library? GetById(int id)
        {
            return _context.Libraries.Find(id);
        }

        public void Add(Library entity)
        {
            _context.Libraries.Add(entity);
            SaveChanges();
        }

        public void Update(Library entity)
        {
            _context.Libraries.Update(entity);
            SaveChanges();
        }

        public void Delete(Library entity)
        {
            _context.Libraries.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges() => _context.SaveChanges();

        public async Task<IEnumerable<Library>> GetAllProducersAsync()
        {
            return await _context.Libraries.ToListAsync();
        }
    }
}

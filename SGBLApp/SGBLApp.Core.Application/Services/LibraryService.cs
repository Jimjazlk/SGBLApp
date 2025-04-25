using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Core.Application.Services
{
    public class LibraryService : IBaseService<LibraryDto>, ILibraryService
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IBookRepository _bookRepository;

        public LibraryService(ILibraryRepository libraryRepository, IBookRepository bookRepository)
        {
            _libraryRepository = libraryRepository;
            _bookRepository = bookRepository;
        }

        public IEnumerable<LibraryDto> GetAll()
        {
            return _libraryRepository.GetAll().Select(l => new LibraryDto
            {
                LibraryId = l.LibraryId,
                Name = l.Name,
                Description = l.Description,
                Location = l.Location
            });
        }

        public LibraryDto? GetById(int id)
        {
            var library = _libraryRepository.GetById(id);
            if (library == null) return null;

            return new LibraryDto
            {
                LibraryId = library.LibraryId,
                Name = library.Name,
                Description = library.Description,
                Location = library.Location
            };
        }

        public void Add(LibraryDto dto)
        {
            var library = new Library
            {
                LibraryId = 0,
                Name = dto.Name ?? string.Empty,
                Description = dto.Description,
                Location = dto.Location
            };
            _libraryRepository.Add(library);
        }

        public void Update(LibraryDto dto)
        {
            if (dto.LibraryId != null)
            {
                var library = new Library
                {
                    LibraryId = dto.LibraryId.Value,
                    Name = dto.Name ?? string.Empty,
                    Description = dto.Description,
                    Location = dto.Location
                };
                _libraryRepository.Update(library);
            }
        }

        public void Delete(int id)
        {
            var library = _libraryRepository.GetById(id);
            if (library != null)
            {
                _libraryRepository.Delete(library);
            }
        }

        public bool HasBooks(int library)
        {
            return _bookRepository.GetAll().Any(b => b.LibraryId == library);
        }
    }
}

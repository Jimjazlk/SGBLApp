using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Core.Application.Services
{
    public class GenreService : IBaseService<GenreDto>, IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IBookRepository _bookRepository;
        public GenreService(IGenreRepository genreRepository, IBookRepository bookRepository)
        {
            _genreRepository = genreRepository;
            _bookRepository = bookRepository;
        }

        public IEnumerable<GenreDto> GetAll()
        {
            return _genreRepository.GetAll().Select(g => new GenreDto
            {
                GenreId = g.GenreId,
                Name = g.Name
            });
        }

        public GenreDto? GetById(int id)
        {
            var genre = _genreRepository.GetById(id);
            if (genre == null) return null;

            return new GenreDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            };
        }

        public void Add(GenreDto dto)
        {
            var genre = new Genre
            {
                GenreId = 0,
                Name = dto.Name
            };
            _genreRepository.Add(genre);
        }

        public void Update(GenreDto dto)
        {
            if (dto.GenreId != null)
            {
                var genre = new Genre
                {
                    GenreId = dto.GenreId.Value,
                    Name = dto.Name
                };
                _genreRepository.Update(genre);
            }
        }

        public void Delete(int id)
        {
            var genre = _genreRepository.GetById(id);
            if (genre != null)
            {
                _genreRepository.Delete(genre);
            }
        }

        public bool HasBooks(int genreId)
        {
            return _bookRepository.GetAll().Any(b => b.PrimaryGenreId == genreId || b.SecondaryGenreId == genreId);
        }
    }
}

using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Core.Application.Services
{
    public class AuthorService : IBaseService<AuthorDto>, IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public IEnumerable<AuthorDto> GetAll()
        {
            return _authorRepository.GetAll().Select(a => new AuthorDto
            {
                AuthorId = a.AuthorId,
                Name = a.Name,
                Biography = a.Biography
            });
        }

        public AuthorDto? GetById(int id)
        {
            var author = _authorRepository.GetById(id);
            if (author == null) return null;

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                Name = author.Name,
                Biography = author.Biography
            };
        }

        public void Add(AuthorDto dto)
        {
            var author = new Author
            {
                AuthorId = 0,
                Name = dto.Name,
                Biography = dto.Biography
            };
            _authorRepository.Add(author);
        }

        public void Update(AuthorDto dto)
        {
            if (dto.AuthorId != null)
            {
                var author = new Author
                {
                    AuthorId = dto.AuthorId.Value,
                    Name = dto.Name,
                    Biography = dto.Biography
                };
                _authorRepository.Update(author);
            }
        }

        public void Delete(int id)
        {
            var author = _authorRepository.GetById(id);
            if (author != null)
            {
                _authorRepository.Delete(author);
            }
        }

        public bool HasBooks(int author)
        {
            return _bookRepository.GetAll().Any(b => b.AuthorId == author);
        }
    }
}

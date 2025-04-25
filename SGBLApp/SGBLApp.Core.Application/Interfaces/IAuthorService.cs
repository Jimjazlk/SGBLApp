using SGBLApp.Core.Application.DTOs;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IAuthorService : IBaseService<AuthorDto>
    {
        bool HasBooks(int id);
    }
}

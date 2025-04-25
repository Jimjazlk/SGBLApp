
using SGBLApp.Core.Application.DTOs;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IGenreService : IBaseService<GenreDto>
    {
        bool HasBooks(int id);
    }
}

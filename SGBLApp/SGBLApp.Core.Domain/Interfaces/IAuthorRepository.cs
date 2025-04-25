
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Domain.Interfaces
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
    }
}

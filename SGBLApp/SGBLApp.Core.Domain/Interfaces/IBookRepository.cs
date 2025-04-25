
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Domain.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<IEnumerable<Book>> SearchBooksAsync(string title, string author, string genre);
        Task<int> GetTotalBookCountAsync();

    }
}

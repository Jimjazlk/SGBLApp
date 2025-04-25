using Microsoft.AspNetCore.Http;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IBookService : IBaseService<BookDto>
    {
        Task<IEnumerable<BookDto>> SearchBooksAsync(string title, string author, string genre);

    }
}

using SGBLApp.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IBookFeedbackService
    {
        Task GiveFeedbackAsync(string userId, int bookId, bool isLiked);
        Task<bool?> GetUserFeedbackAsync(string userId, int bookId);
        Task<List<int>> GetDislikedBookIdsAsync(string userId);
        Task<List<BookDto>> GetLikedBooksAsync(string userId);
        Task<List<BookDto>> GetDislikedBooksAsync(string userId);

    }
}

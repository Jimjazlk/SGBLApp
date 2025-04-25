
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface IRecommendationService
    {
        Task<IEnumerable<Book>> GetUserRecommendationsAsync(string userId);
        Task<IEnumerable<Book>> GetPopularBooksAsync(int? days = null);
    }
}

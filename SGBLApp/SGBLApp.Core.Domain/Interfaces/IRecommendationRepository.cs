

using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Domain.Interfaces
{
    public interface IRecommendationRepository
    {
        Task<IEnumerable<Book>> GetBooksByGenresAsync(IEnumerable<int> genreIds, int limit);
        Task<IEnumerable<string>> GetSimilarUsersAsync(string userId, int minCommonGenres);
        Task<IEnumerable<Book>> GetPopularBooksAsync(int? days = null, int limit = 10);
        Task<IEnumerable<Book>> GetCollaborativeFilteringBooksAsync(IEnumerable<string> similarUserIds);
    }
}

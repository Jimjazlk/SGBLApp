using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBLApp.Core.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookFeedbackService _feedbackService;

        public RecommendationService(
            IRecommendationRepository recommendationRepository,
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IBookFeedbackService feedbackService)
        {
            _recommendationRepository = recommendationRepository;
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _feedbackService = feedbackService;
        }

        public async Task<IEnumerable<Book>> GetUserRecommendationsAsync(string userId)
        {
            var userLoans = await _loanRepository.GetLoansByUserAsync(userId);
            var borrowedBookIds = userLoans.Select(l => l.BookId).Distinct().ToList();

            if (!userLoans.Any())
                return Enumerable.Empty<Book>();

            var userGenreIds = GetUserGenreIds(userLoans);
            var contentBased = await _recommendationRepository.GetBooksByGenresAsync(userGenreIds, 15);
            var similarUsers = await _recommendationRepository.GetSimilarUsersAsync(userId, 2);
            var collaborative = await _recommendationRepository.GetCollaborativeFilteringBooksAsync(similarUsers);
            var dislikedBookIds = await _feedbackService.GetDislikedBookIdsAsync(userId);

            var recommendations = contentBased
                .Concat(collaborative)
                .Where(b => !borrowedBookIds.Contains(b.BookId) &&
                            !dislikedBookIds.Contains(b.BookId))
                .DistinctBy(b => b.BookId)
                .OrderByDescending(b => CalculateBookScore(b.BookPopularity))
                .Take(10);

            return await ApplyFallbackStrategy(recommendations, 3);
        }

        private List<int> GetUserGenreIds(IEnumerable<Loan> loans)
        {
            return loans
                .SelectMany(l => new[] { l.Book?.PrimaryGenreId, l.Book?.SecondaryGenreId })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();
        }

        public async Task<IEnumerable<Book>> GetPopularBooksAsync(int? days = null)
        {
            var books = await _recommendationRepository.GetPopularBooksAsync(days);
            return books?.DistinctBy(b => b.BookId) ?? Enumerable.Empty<Book>();
        }

        private int CalculateBookScore(BookPopularity popularity)
        {
            if (popularity == null) return 0;

            return (popularity.LoanCount * 3) +
                   (popularity.ReservationCount * 2) +
                   (popularity.ClickCount / 2) +
                   (popularity.RecommendationFeedbackPositive * 5) -
                   (popularity.RecommendationFeedbackNegative * 3);
        }

        private async Task<IEnumerable<Book>> ApplyFallbackStrategy(IEnumerable<Book> recommendations, int minItems)
        {
            var list = recommendations.ToList();
            if (list.Count >= minItems) return list;

            var popularBooks = await _recommendationRepository.GetPopularBooksAsync(30);
            return list
                .Union(popularBooks)
                .DistinctBy(b => b.BookId)
                .Take(minItems);
        }

        private async Task<IEnumerable<Book>> GetDiversifiedRecommendations(IEnumerable<Book> baseRecommendations)
        {
            var allBooksCount = await _bookRepository.GetTotalBookCountAsync();

            if (allBooksCount <= 10)
            {
                return await _recommendationRepository.GetPopularBooksAsync(null, 5);
            }

            return baseRecommendations;
        }
    }
}

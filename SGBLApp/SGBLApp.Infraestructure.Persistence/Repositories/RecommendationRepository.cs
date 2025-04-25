using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationContext _context;

        public RecommendationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksByGenresAsync(IEnumerable<int> genreIds, int limit)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.PrimaryGenre)
                .Where(b => genreIds.Contains(b.PrimaryGenreId) ||
                            (b.SecondaryGenreId != null && genreIds.Contains(b.SecondaryGenreId.Value)))
                .OrderByDescending(b => b.BookPopularity.LoanCount +
                                        b.BookPopularity.ReservationCount +
                                        (b.BookPopularity.ClickCount / 2))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetSimilarUsersAsync(string userId, int minCommonGenres)
        {
            // Obtener géneros que ha leído el usuario actual
            var userGenreIds = await _context.Loans
                .Where(l => l.UserId == userId)
                .Select(l => l.Book.PrimaryGenreId)
                .Distinct()
                .ToListAsync();

            // Buscar otros usuarios que compartan géneros en común
            var similarUsers = await _context.Loans
                .Where(l => l.UserId != userId && userGenreIds.Contains(l.Book.PrimaryGenreId))
                .GroupBy(l => l.UserId)
                .Where(g => g.Select(x => x.Book.PrimaryGenreId).Distinct().Count() >= minCommonGenres)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(5)
                .ToListAsync();

            return similarUsers;
        }

        public async Task<IEnumerable<Book>> GetCollaborativeFilteringBooksAsync(IEnumerable<string> similarUserIds)
        {
            return await _context.Loans
                .Include(l => l.Book)
                    .ThenInclude(b => b.Author)
                .Where(l => similarUserIds.Contains(l.UserId))
                .Select(l => l.Book)
                .Distinct()
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetPopularBooksAsync(int? days = null, int limit = 10)
        {
            var query = _context.Books
                .Include(b => b.BookPopularity)
                .Include(b => b.Author)
                .Include(b => b.PrimaryGenre)
                .AsQueryable();

            if (days.HasValue)
            {
                var cutoffDate = DateTime.Now.AddDays(-days.Value);
                query = query.Where(b => b.Loans.Any(l => l.LoanDate >= cutoffDate));
            }

            return await query
                .OrderByDescending(b => b.BookPopularity.LoanCount +
                                        b.BookPopularity.ReservationCount +
                                        (b.BookPopularity.ClickCount / 2))
                .Take(limit)
                .ToListAsync();
        }
    }
}

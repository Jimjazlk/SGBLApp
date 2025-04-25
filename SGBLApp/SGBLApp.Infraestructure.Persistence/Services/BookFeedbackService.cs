using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Infraestructure.Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBLApp.Infrastructure.Persistence.Services
{
    public class BookFeedbackService : IBookFeedbackService
    {
        private readonly ApplicationContext _context;

        public BookFeedbackService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task GiveFeedbackAsync(string userId, int bookId, bool isLiked)
        {
            var feedback = await _context.BookFeedbacks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

            if (feedback == null)
            {
                feedback = new BookFeedback
                {
                    UserId = userId,
                    BookId = bookId,
                    IsLiked = isLiked,
                    Date = DateTime.UtcNow
                };
                _context.BookFeedbacks.Add(feedback);
            }
            else if (feedback.IsLiked == isLiked)
            {
                _context.BookFeedbacks.Remove(feedback); // toggle
            }
            else
            {
                feedback.IsLiked = isLiked;
                feedback.Date = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool?> GetUserFeedbackAsync(string userId, int bookId)
        {
            return await _context.BookFeedbacks
                .Where(f => f.UserId == userId && f.BookId == bookId)
                .Select(f => (bool?)f.IsLiked)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetDislikedBookIdsAsync(string userId)
        {
            return await _context.BookFeedbacks
                .Where(f => f.UserId == userId && f.IsLiked == false)
                .Select(f => f.BookId)
                .ToListAsync();
        }

        public async Task<List<BookDto>> GetLikedBooksAsync(string userId)
        {
            return await _context.BookFeedbacks
                .Where(f => f.UserId == userId && f.IsLiked == true)
                .Include(f => f.Book).ThenInclude(b => b.Author)
                .Include(f => f.Book).ThenInclude(b => b.PrimaryGenre)
                .Select(f => new BookDto
                {
                    BookId = f.BookId,
                    Title = f.Book.Title,
                    ISBN = f.Book.ISBN, // ✅ NECESARIO para evitar error
                    AuthorName = f.Book.Author.Name,
                    PrimaryGenreName = f.Book.PrimaryGenre.Name,
                    ImageUrl = f.Book.ImageUrl,
                    PublicationYear = f.Book.PublicationYear,
                    UserLiked = true
                })
                .ToListAsync();
        }

        public async Task<List<BookDto>> GetDislikedBooksAsync(string userId)
        {
            return await _context.BookFeedbacks
                .Where(f => f.UserId == userId && f.IsLiked == false)
                .Include(f => f.Book).ThenInclude(b => b.Author)
                .Include(f => f.Book).ThenInclude(b => b.PrimaryGenre)
                .Select(f => new BookDto
                {
                    BookId = f.BookId,
                    Title = f.Book.Title,
                    ISBN = f.Book.ISBN, // ✅ NECESARIO también aquí
                    AuthorName = f.Book.Author.Name,
                    PrimaryGenreName = f.Book.PrimaryGenre.Name,
                    ImageUrl = f.Book.ImageUrl,
                    PublicationYear = f.Book.PublicationYear,
                    UserLiked = false
                })
                .ToListAsync();
        }

    }
}

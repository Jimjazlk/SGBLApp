
using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;
using SGBLApp.Infraestructure.Persistence.Context;

namespace SGBLApp.Infraestructure.Persistence.Repositories
{
    public class LoanRepository : IBaseRepository<Loan>, ILoanRepository
    {
        private readonly ApplicationContext _context;

        public LoanRepository(ApplicationContext context)
        {
            _context = context;
        }

        #region IBaseRepository Implementation
        public IEnumerable<Loan> GetAll()
        {
            return _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToList();
        }

        public Loan? GetById(int id)
        {
            return _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefault(l => l.LoanId == id);
        }

        public void Add(Loan entity)
        {
            _context.Loans.Add(entity);
            SaveChanges();
        }

        public void Update(Loan entity)
        {
            _context.Update(entity);
            SaveChanges();
        }

        public void Delete(Loan entity)
        {
            _context.Loans.Remove(entity);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        #endregion

        public async Task<IEnumerable<Loan>> GetLoansByUserAsync(string userId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(string userId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Where(l => l.UserId == userId &&
                           l.Status == LoanStatus.Aprobado &&
                           l.ReturnDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Aprobado && l.ReturnDate == null)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            var today = DateTime.Today;
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Aprobado && l.ReturnDate == null && l.DueDate < today)
                .OrderBy(l => l.DueDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveLoansCountByUserAsync(string userId)
        {
            return await _context.Loans
            .CountAsync(l => l.UserId == userId &&
                             l.Status == LoanStatus.Aprobado &&
                             l.ReturnDate == null);
        }

        public async Task<bool> HasOverdueLoansAsync(string userId)
        {
            return await _context.Loans
            .AnyAsync(l => l.UserId == userId &&
                     l.Status == LoanStatus.Aprobado &&
                     l.DueDate < DateTime.Today &&
                     l.ReturnDate == null);
        }

        public async Task<Loan> GetLoanDetailsAsync(int loanId)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);
        }

        public async Task<IEnumerable<Loan>> GetLoansDueSoonAsync(DateTime cutoffDate)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.DueDate <= cutoffDate && l.Status == LoanStatus.Aprobado)
                .ToListAsync();
        }

        public async Task<bool> HasActiveLoanForBookAsync(string userId, int bookId)
        {
            return await _context.Loans
                .AnyAsync(l => l.UserId == userId &&
                              l.BookId == bookId &&
                              l.Status == LoanStatus.Aprobado &&
                              l.ReturnDate == null);
        }
    }
}


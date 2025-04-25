
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Core.Domain.Interfaces
{
    public interface ILoanRepository : IBaseRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetLoansByUserAsync(string userId);
        Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(string userId);
        Task<IEnumerable<Loan>> GetActiveLoansAsync();
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task<int> GetActiveLoansCountByUserAsync(string userId);
        Task<bool> HasOverdueLoansAsync(string userId);
        Task<Loan> GetLoanDetailsAsync(int loanId);
        Task<IEnumerable<Loan>> GetLoansDueSoonAsync(DateTime cutoffDate);
        Task<bool> HasActiveLoanForBookAsync(string userId, int bookId);
    }
}

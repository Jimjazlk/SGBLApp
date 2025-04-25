using AutoMapper.Internal;
using SGBLApp.Core.Application.DTOs.Loans;
using SGBLApp.Core.Application.Utilities;

namespace SGBLApp.Core.Application.Interfaces
{
    public interface ILoanService
    {
        // Métodos básicos
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<LoanDto> GetLoanByIdAsync(int id);

        // Métodos para usuarios
        Task<IEnumerable<LoanDto>> GetLoansByUserAsync(string userId);
        Task<(bool canRequest, string message)> CanRequestLoanAsync(string userId);
        Task<(bool success, string message)> RequestLoanAsync(LoanRequestDto loanRequest);

        // Métodos para bibliotecarios/administradores
        Task<IEnumerable<LoanDto>> GetPendingLoansAsync();
        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
        Task<(bool success, string message)> ApproveLoanAsync(int loanId);
        Task<(bool success, string message)> RejectLoanAsync(int loanId, string reason);

        // Métodos para devoluciones
        Task<Result> ReturnBookAsync(LoanReturnDto returnDto);
        // Configuración
        int MaxLoansPerUser { get; }
        int DefaultLoanDurationDays { get; }
    }
}

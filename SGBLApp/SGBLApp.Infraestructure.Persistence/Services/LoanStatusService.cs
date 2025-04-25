using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SGBLApp.Core.Domain.Enum;
using SGBLApp.Core.Domain.Interfaces;

namespace SGBLApp.Infraestructure.Persistence.Services
{
    public class LoanStatusService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public LoanStatusService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var loanRepo = scope.ServiceProvider.GetRequiredService<ILoanRepository>();

                var overdueLoans = await loanRepo.GetOverdueLoansAsync();
                foreach (var loan in overdueLoans)
                {
                    loan.Status = LoanStatus.Vencido;
                    loanRepo.Update(loan);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Ejecutar diariamente
            }
        }
    }
}

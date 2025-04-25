namespace SGBLApp.Core.Application.Settings
{
    public class LoanSettings
    {
        public int MaxLoansPerUser { get; set; } = 3; // Valor por defecto
        public int DefaultLoanDurationDays { get; set; } = 21; // Valor por defecto
    }
}

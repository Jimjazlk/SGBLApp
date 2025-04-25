namespace SGBLApp.Core.Application.Settings
{
    public class EmailSettings
    {
        public required string SmtpServer { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FromAddress { get; set; }
        public required string SenderName { get; set; }
    }
}

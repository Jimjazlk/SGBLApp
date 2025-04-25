
namespace SGBLApp.Core.Application.DTOs.User
{
    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}


using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGBLApp.Core.Application.DTOs.User
{
    public class UserRoleDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}

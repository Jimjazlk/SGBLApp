using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Core.Application.DTOs
{
    public class AuthorDto
    {
        public int? AuthorId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Name { get; set; }
        public string? Biography { get; set; }
    }
}

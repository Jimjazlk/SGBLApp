
using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Core.Application.DTOs
{
    public class GenreDto
    {
        public int? GenreId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Name { get; set; }
    }
}

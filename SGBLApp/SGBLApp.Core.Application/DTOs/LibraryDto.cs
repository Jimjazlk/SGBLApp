
using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Core.Application.DTOs
{
    public class LibraryDto
    {
        public int? LibraryId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}

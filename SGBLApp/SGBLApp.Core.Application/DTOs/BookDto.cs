
using SGBLApp.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGBLApp.Core.Application.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }

        public bool? UserLiked { get; set; }


        [Required(ErrorMessage = "El título es requerido")]
        public string Title { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "El ISBN es requerido")]
        [RegularExpression(@"^(?:\d{9}[\dXx]|\d{13})$", ErrorMessage = "Formato ISBN inválido")]
        public required string ISBN { get; set; }

        [Required(ErrorMessage = "El año de publicación es requerido")]
        [Range(1000, 9999, ErrorMessage = "Año inválido")]
        public int PublicationYear { get; set; }

        [Required(ErrorMessage = "La imagen es requerida")]
        [Url(ErrorMessage = "Debe de ser un URL Valido")]
        public required string ImageUrl { get; set; }

        public BookStatus Status { get; set; }
        public int? PopularityScore { get; set; }

        [Required(ErrorMessage = "El número de copias es requerido")]
        [Range(1, 100, ErrorMessage = "Debe tener entre 1 y 100 copias")]
        public int Copies { get; set; }

        public int AvailableCopies { get; set; }


        // LIBRARY
        [Required(ErrorMessage = "Debe de seleccionar una librería")]
        public int LibraryId { get; set; }
        public List<SelectListItem>? Libraries { get; set; }
        public string? LibraryName { get; set; }


        // AUTHOR
        [Required(ErrorMessage = "Debe de seleccionar un autor.")]
        public int AuthorId{ get; set; }
        public List<SelectListItem>? Authors { get; set; }
        public string? AuthorName { get; set; }


        // GENRE
        [Required(ErrorMessage = "Debe de seleccionar un genero primario.")]
        public int PrimaryGenreId { get; set; }
        public List<SelectListItem>? PrimaryGenres { get; set; }
        public string? PrimaryGenreName { get; set; }

        public int? SecondaryGenreId { get; set; }
        public List<SelectListItem>? SecondaryGenres { get; set; }
        public string? SecondaryGenreName { get; set; }


        // RECOMMENDATIONS
        [Display(Name = "Motivo de Recomendación")]
        public string? RecommendationReason { get; set; } // Ej: "Por tus préstamos de Ciencia Ficción"

        [Display(Name = "Puntuación de Relevancia")]
        public int? RecommendationScore { get; set; }

        #region Validación personalizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AvailableCopies > Copies)
            {
                yield return new ValidationResult(
                    "Las copias disponibles no pueden ser mayores al total",
                    new[] { nameof(AvailableCopies) });
            }
        }
        #endregion
    }
}

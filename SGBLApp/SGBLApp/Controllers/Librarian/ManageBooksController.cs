using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace SGBLApp.Controllers.Librarian
{
    [Authorize(Policy = "LibrarianOnly")]
    public class ManageBooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly ILibraryService _libraryService;


        public ManageBooksController
            (IBookService bookService,
            IAuthorService authorService,
            IGenreService genreService,
            ILibraryService libraryService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _genreService = genreService;
            _libraryService = libraryService;
        }

        public IActionResult Index()
        {
            var books = _bookService.GetAll();
            return View("~/Views/Manage/Book/Index.cshtml", books);
        }

        public IActionResult Details(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null) return NotFound();
            return View("~/Views/Manage/Book/Details.cshtml", book);
        }

        public IActionResult Create()
        {
            var dto = new BookDto
            {
                ISBN = string.Empty,
                ImageUrl = string.Empty,
                Authors = GetAuthors(),
                Libraries = GetLibraries(),
                PrimaryGenres = GetPrimaryGenres(),
                SecondaryGenres = GetSecondaryGenres()
            };
            return View("~/Views/Manage/Book/Create.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookDto book)
        {

            if (!ModelState.IsValid)
            {
                book.Authors = GetAuthors();
                book.Libraries = GetLibraries();
                book.PrimaryGenres = GetPrimaryGenres();
                book.SecondaryGenres = GetSecondaryGenres();
                return View(book);
            }

            _bookService.Add(book);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var bookDto = _bookService.GetById(id);
            if (bookDto == null) return NotFound();

            // Cargar listas directamente en el DTO
            bookDto.Libraries = GetLibraries();
            bookDto.Authors = GetAuthors();
            bookDto.PrimaryGenres = GetPrimaryGenres();
            bookDto.SecondaryGenres = GetSecondaryGenres();

            return View("~/Views/Manage/Book/Edit.cshtml", bookDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookDto book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recargar listas antes de mostrar errores
                    book.Libraries = GetLibraries();
                    book.Authors = GetAuthors();
                    book.PrimaryGenres = GetPrimaryGenres();
                    book.SecondaryGenres = GetSecondaryGenres();
                    return View("~/Views/Manage/Book/Edit.cshtml", book);
                }

                _bookService.Update(book);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Recargar listas y mostrar error
                ModelState.AddModelError("", ex.Message);
                book.Libraries = GetLibraries();
                book.Authors = GetAuthors();
                book.PrimaryGenres = GetPrimaryGenres();
                book.SecondaryGenres = GetSecondaryGenres();
                return View("~/Views/Manage/Book/Edit.cshtml", book);
            }
        }

        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null) return NotFound();
            return View("~/Views/Manage/Book/Delete.cshtml", book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _bookService.Delete(id);
            return RedirectToAction(nameof(Index));
        }


        private List<SelectListItem> GetLibraries()
        {
            var library = _libraryService.GetAll();
            return library.Select(p => new SelectListItem
            {
                Value = p.LibraryId.ToString(),
                Text = p.Name
            }).ToList();
        }

        private List<SelectListItem> GetAuthors()
        {
            var authors = _authorService.GetAll();
            return authors.Select(p => new SelectListItem
            {
                Value = p.AuthorId.ToString(),
                Text = p.Name
            }).ToList();
        }

        private List<SelectListItem> GetPrimaryGenres()
        {
            var genres = _genreService.GetAll();
            return genres.Select(g => new SelectListItem
            {
                Value = g.GenreId.ToString(),
                Text = g.Name
            }).ToList();
        }

        private List<SelectListItem> GetSecondaryGenres()
        {
            var genres = _genreService.GetAll();
            return genres.Select(g => new SelectListItem
            {
                Value = g.GenreId.ToString(),
                Text = g.Name
            }).ToList();
        }
    }
}
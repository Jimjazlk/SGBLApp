using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;

namespace SGBLApp.Controllers.Librarian
{
    [Authorize(Policy = "LibrarianOnly")]
    public class ManageAuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public ManageAuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public IActionResult Index()
        {
            var authors = _authorService.GetAll();
            return View("~/Views/Manage/Author/Index.cshtml", authors);
        }

        public IActionResult Create()
        {
            return View("~/Views/Manage/Author/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(AuthorDto authorDto)
        {
            if (ModelState.IsValid)
            {
                _authorService.Add(authorDto);
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Manage/Author/Create.cshtml", authorDto);

        }

        public IActionResult Edit(int id)
        {
            var author = _authorService.GetById(id);
            return View("~/Views/Manage/Author/Edit.cshtml", author);
        }

        [HttpPost]
        public IActionResult Edit(int id, AuthorDto authorDto)
        {
            if (ModelState.IsValid)
            {
                return View("~/Views/Manage/Author/Edit.cshtml", authorDto);
            }
            _authorService.Update(authorDto);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var author = _authorService.GetById(id);
            return author == null
                ? NotFound()
                : View("~/Views/Manage/Author/Delete.cshtml", author);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_authorService.HasBooks(id))
            {

                TempData["ErrorMessage"] = "No puedes eliminar un autor que contiene libros.";
                return RedirectToAction(nameof(Index));
            }
            _authorService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

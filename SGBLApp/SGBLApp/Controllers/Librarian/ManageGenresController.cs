using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Services;

namespace SGBLApp.Controllers.Librarian
{
    [Authorize(Policy = "LibrarianOnly")]
    public class ManageGenresController : Controller
    {
        private readonly IGenreService _genreService;

        public ManageGenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public IActionResult Index()
        {
            var genres = _genreService.GetAll();
            return View("~/Views/Manage/Genre/Index.cshtml", genres);
        }


        public IActionResult Create()
        {
            return View("~/Views/Manage/Genre/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GenreDto genre)
        {
            if (ModelState.IsValid)
            {
                _genreService.Add(genre);
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Manage/Genre/Create.cshtml", genre);
        }

        public IActionResult Edit(int id)
        {
            var genre = _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View("~/Views/Manage/Genre/Edit.cshtml", genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GenreDto genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _genreService.Update(genre);
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Manage/Genre/Edit.cshtml", genre);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var genre = _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }


            if (_genreService.HasBooks(id))
            {

                TempData["ErrorMessage"] = "No puedes eliminar un genero que contiene series.";
                return RedirectToAction(nameof(Index));
            }


            _genreService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

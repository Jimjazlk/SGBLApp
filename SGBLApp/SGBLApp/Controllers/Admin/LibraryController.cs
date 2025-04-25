using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Services;

namespace SGBLApp.Controllers.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class LibraryController : Controller
    {
        private readonly ILibraryService _libraryService;

        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IActionResult Index()
        {
            var libraries = _libraryService.GetAll();
            return View("~/Views/Admin/Library/Index.cshtml", libraries);
        }

        public IActionResult Create()
        {
            return View("~/Views/Admin/Library/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(LibraryDto libraryDto)
        {
            if (ModelState.IsValid)
            {
                return View("~/Views/Admin/Library/Create.cshtml", libraryDto);
            }
            _libraryService.Add(libraryDto);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var library = _libraryService.GetById(id);
            return View("~/Views/Admin/Library/Edit.cshtml", library);
        }

        [HttpPost]
        public IActionResult Edit(int id, LibraryDto libraryDto)
        {
            if (ModelState.IsValid)
            {
                return View("~/Views/Admin/Library/Edit.cshtml", libraryDto);
            }
            _libraryService.Update(libraryDto);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var library = _libraryService.GetById(id);
            return library == null
                ? NotFound()
                : View("~/Views/Admin/Library/Delete.cshtml", library);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _libraryService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

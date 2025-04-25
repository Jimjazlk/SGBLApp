using Microsoft.AspNetCore.Mvc;

namespace SGBLApp.Controllers.Librarian
{
    public class LibrarianController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Manage/Index.cshtml");
        }
    }
}

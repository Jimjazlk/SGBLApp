using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using System.Threading.Tasks;

namespace SGBLApp.Controllers
{
    [AllowAnonymous]
    public class CatalogController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IBookFeedbackService _feedbackService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CatalogController(
            IBookService bookService,
            IBookFeedbackService feedbackService,
            UserManager<ApplicationUser> userManager)
        {
            _bookService = bookService;
            _feedbackService = feedbackService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string title, string author, string genre)
        {
            var books = await _bookService.SearchBooksAsync(title, author, genre);

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                foreach (var book in books)
                {
                    var feedback = await _feedbackService.GetUserFeedbackAsync(user.Id, book.BookId);
                    book.UserLiked = feedback;
                }
            }

            return View("~/Views/Catalog/Index.cshtml", books);
        }


        public IActionResult Details(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View("~/Views/Catalog/Details.cshtml", book);
        }
    }
}

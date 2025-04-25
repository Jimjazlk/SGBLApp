using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs;
using SGBLApp.Core.Application.DTOs.Loans;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Domain.Entities;
using SGBLApp.Core.Domain.Enum;

namespace SGBLApp.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanController(
            ILoanService loanService,
            IBookService bookService,
            UserManager<ApplicationUser> userManager)
        {
            _loanService = loanService;
            _bookService = bookService;
            _userManager = userManager;
        }

        // Vista general - diferentes según el rol
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(); // O RedirectToAction("Login", "Account")

            if (User.IsInRole("Admin") || User.IsInRole("Librarian"))
            {
                var allLoans = await _loanService.GetAllLoansAsync();
                return View("~/Views/Loan/AdminIndex.cshtml", allLoans);
            }
            else
            {
                var userLoans = await _loanService.GetLoansByUserAsync(user.Id);
                return View("~/Views/Loan/UserIndex.cshtml", userLoans);
            }
        }

        // Solicitud de préstamo (para usuarios)
        [HttpGet]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Request(int bookId)
        {
            var book = _bookService.GetById(bookId);
            if (book == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var (canRequest, message) = await _loanService.CanRequestLoanAsync(user.Id);

            if (!canRequest)
            {
                TempData["ErrorMessage"] = message;
                return RedirectToAction("Details", "Catalog", new { id = bookId });
            }

            var model = new LoanRequestDto
            {
                BookId = book.BookId,
                BookTitle = book.Title,
                UserId = user.Id,
                UserName = user.Name,
                DueDate = DateTime.Today.AddDays(_loanService.DefaultLoanDurationDays)
            };

            return View("~/Views/Loan/Request.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> ConfirmRequest(LoanRequestDto model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Loan/Request.cshtml", model);

            var result = await _loanService.RequestLoanAsync(model);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
                return View("~/Views/Loan/Request.cshtml", model);
            }
        }

        // Aprobación/rechazo de préstamos (para bibliotecarios y admin)
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Pending()
        {
            var pendingLoans = await _loanService.GetPendingLoansAsync();
            return View("~/Views/Loan/Pending.cshtml", pendingLoans);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _loanService.ApproveLoanAsync(id);

            if (result.success)
                TempData["SuccessMessage"] = result.message;
            else
                TempData["ErrorMessage"] = result.message;

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var result = await _loanService.RejectLoanAsync(id, reason);

            if (result.success)
                TempData["SuccessMessage"] = result.message;
            else
                TempData["ErrorMessage"] = result.message;

            return RedirectToAction(nameof(Pending));
        }

        // Devolución de libros
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null || loan.Status != LoanStatus.Aprobado || loan.ReturnDate.HasValue)
                return NotFound();

            var model = new LoanReturnDto
            {
                LoanId = loan.LoanId,
                BookTitle = loan.BookTitle,
                UserName = loan.UserName,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate
            };

            return View("~/Views/Loan/Return.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReturn(LoanReturnDto model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Loan/Return.cshtml", model);

            var result = await _loanService.ReturnBookAsync(model);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View("~/Views/Loan/Return.cshtml", model);
            }
        }

        // Préstamos vencidos (para admin y bibliotecarios)
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Overdue()
        {
            var overdueLoans = await _loanService.GetOverdueLoansAsync();
            return View("~/Views/Loan/Overdue.cshtml", overdueLoans);
        }
    }
}
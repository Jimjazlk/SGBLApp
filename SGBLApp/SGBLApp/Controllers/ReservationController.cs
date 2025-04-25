using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs.Reservation;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Services;
using SGBLApp.Core.Domain.Entities;
using System.Security.Claims;

namespace SGBLApp.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IBookService _bookService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(IReservationService reservationService, IBookService bookService, UserManager<ApplicationUser> userManager)
        {
            _reservationService = reservationService;
            _bookService = bookService;
            _userManager = userManager;
        }

        // GET: Reservations/Create
        public IActionResult Create(int bookId)
        {
            // Validar bookId
            if (bookId <= 0) return NotFound();

            // Inicializar DTO con bookId
            var dto = new ReservationRequestDto { BookId = bookId };

            // Cargar datos del libro para la vista
            LoadBookData(bookId);

            return View(dto);
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationRequestDto request)
        {
            if (request.BookId <= 0)
            {
                TempData["ErrorMessage"] = "Libro inválido";
                return RedirectToAction("Details", "Catalog", new { id = request.BookId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para reservar";
                return RedirectToAction("Details", "Catalog", new { id = request.BookId });
            }

            request.UserId = user.Id;
            var result = await _reservationService.CreateReservationAsync(request);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "¡Reserva creada exitosamente!";
                return RedirectToAction("Details", "Catalog", new { id = request.BookId });
            }

            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("Details", "Catalog", new { id = request.BookId });
        }


        // GET: Reservations/MyReservations
        public async Task<IActionResult> MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return View(reservations);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _reservationService.GetReservationDetailsAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return View(result.Value);
        }

        // POST: Reservations/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _reservationService.CancelReservationAsync(id);
            return RedirectToAction(nameof(MyReservations));
        }

        // POST: Reservations/Fulfill/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> Fulfill(int id)
        {
            var result = await _reservationService.FulfillReservationAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Reserva cumplida exitosamente";
            return RedirectToAction(nameof(Details), new { id });
        }

        private void LoadBookData(int bookId)
        {
            var book = _bookService.GetById(bookId);
            ViewBag.BookTitle = book?.Title ?? "Libro no encontrado";
            ViewBag.AuthorName = book?.AuthorName ?? "Autor desconocido";
            ViewBag.LibraryName = book?.LibraryName ?? "Biblioteca no especificada";
        }

    }
}

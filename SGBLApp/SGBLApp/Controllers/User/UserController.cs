using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGBLApp.Core.Application.DTOs.User;
using SGBLApp.Core.Application.Interfaces;
using SGBLApp.Core.Application.Services;
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Controllers.User
{
    [Authorize(Policy = "UserOnly")]
    public class UserController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoanService _loanService;
        private readonly IReservationService _reservationService;

        public UserController(INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            ILoanService loanService,
            IReservationService reservationService)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _loanService = loanService;
            _reservationService = reservationService;
        }


        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileDto
            {
                Name = user.Name,
                Email = user.Email,
                ActiveLoans = await _loanService.GetLoansByUserAsync(user.Id),
                ActiveReservations = await _reservationService.GetUserReservationsAsync(user.Id)
            };

            return View(model);
        }

        public IActionResult MyLoans()
        {
            return View();
        }

        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Notifications()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return View("~/Views/User/Notifications.cshtml", notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return RedirectToAction(nameof(Notifications));
        }
    }
}

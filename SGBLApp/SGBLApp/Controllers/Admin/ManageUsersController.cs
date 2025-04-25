using Microsoft.AspNetCore.Mvc;

namespace SGBLApp.Controllers.Admin
{
    public class ManageUsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

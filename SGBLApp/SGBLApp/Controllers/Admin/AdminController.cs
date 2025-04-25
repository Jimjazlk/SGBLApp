using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGBLApp.Core.Application.DTOs.User;
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Controllers.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();
            var model = new List<UserRoleDto>();

            foreach (var user in users)
            {
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault(); // Solo un rol

                model.Add(new UserRoleDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Roles = roles.Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Name,
                        Selected = r.Name == userRole
                    }).ToList(),
                    SelectedRole = userRole
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUsers(List<UserRoleDto> model)
        {
            if (ModelState.IsValid)
            {
                foreach (var userModel in model)
                {
                    var user = await _userManager.FindByIdAsync(userModel.UserId);
                    if (user == null) continue;

                    // Obtener roles actuales
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = currentRoles.FirstOrDefault();

                    // Solo actualizar si el rol cambió
                    if (userModel.SelectedRole != currentRole)
                    {
                        // Eliminar roles actuales
                        if (currentRoles.Any())
                        {
                            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            if (!removeResult.Succeeded)
                            {
                                ModelState.AddModelError("", $"Error eliminando roles de {user.Email}");
                                continue;
                            }
                        }

                        // Agregar nuevo rol si se seleccionó uno
                        if (!string.IsNullOrEmpty(userModel.SelectedRole))
                        {
                            if (!await _roleManager.RoleExistsAsync(userModel.SelectedRole))
                            {
                                ModelState.AddModelError("", $"El rol {userModel.SelectedRole} no existe");
                                continue;
                            }

                            var addResult = await _userManager.AddToRoleAsync(user, userModel.SelectedRole);
                            if (!addResult.Succeeded)
                            {
                                ModelState.AddModelError("", $"Error asignando rol a {user.Email}: {string.Join(", ", addResult.Errors)}");
                            }
                        }
                    }
                }

                if (ModelState.ErrorCount == 0)
                {
                    return RedirectToAction("ManageUsers");
                }
            }

            // Reconstruir Roles para la vista si hay errores
            var allRoles = _roleManager.Roles.ToList();
            foreach (var userDto in model)
            {
                userDto.Roles = allRoles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = r.Name == userDto.SelectedRole
                }).ToList();
            }

            return View(model);
        }

    }
}

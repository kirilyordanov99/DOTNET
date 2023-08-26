using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{//admin panel page
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(int page = 1, string searchTerm = "")
        {
            int pageSize = 10; // Set your desired page size
            var allUsers = _userManager.Users;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allUsers = allUsers.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            int totalUsers = allUsers.Count();
            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            var users = allUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new SearchUsersViewModel
            {
                SearchTerm = searchTerm,
                Users = users.Select(u => new UsersViewModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault()
                }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            var viewModel = new UsersViewModel
            {
                AvailableRoles = new SelectList(roles)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            model.AvailableRoles = new SelectList(roles);
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _userManager.GetRolesAsync(user).Result;
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new UsersViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                SelectedRole = userRoles.FirstOrDefault(),
                AvailableRoles = new SelectList(roles)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByIdAsync(model.Id).Result;
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.Username;
                user.Email = model.Email;

                await _userManager.UpdateAsync(user);

                // Update user's role
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                return RedirectToAction("Index");
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            model.AvailableRoles = new SelectList(roles);
            return View(model);
        }

        public IActionResult Details(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }

            var userRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var viewModel = new UsersViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = userRole
            };

            return View(viewModel);
        }

        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null)
            {
                return NotFound();
            }

            var userRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var viewModel = new UsersViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = userRole
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View("Error");
        }
    }
}

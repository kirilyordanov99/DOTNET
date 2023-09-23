using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_Application.Models;

namespace Web_Application.Controllers
{//admin panel page
    [Authorize(Roles = "Admin")]
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
            int pageSize = 10;
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            var users = usersQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var userViewModels = users.Select(u => new UsersViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                userRole = _userManager.GetRolesAsync(u).Result.ToList() // Store roles in a List<string>
            }).ToList();
            ;

            var viewModel = new SearchUsersViewModel
            {
                Users = userViewModels,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)usersQuery.Count() / pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CreateRoles()
        {
            var roles = new List<string> { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var newRole = new IdentityRole { Name = role };
                    await _roleManager.CreateAsync(newRole);
                }
            }

            return RedirectToAction("Index");
        }

        // Add the ListRoles action to display a list of roles
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // Continue with the rest of your actions...

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
                var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
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

            // Load available roles for the dropdown
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
                UserName = user.UserName,
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
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;

                await _userManager.UpdateAsync(user);

                // Update user's role
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    // Remove current roles and assign the new one
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                return RedirectToAction("Index");
            }

            // Load available roles for the dropdown
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
                UserName = user.UserName,
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
                UserName = user.UserName,
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

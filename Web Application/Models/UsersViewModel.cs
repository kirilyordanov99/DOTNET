using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web_Application.Models
{//admin panel page
    public class UsersViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneNumberConfirmed { get; set; }
        public string? TwoFactorEnabled { get; set; }
        public string? LockoutEnd { get; set; }

        public List<string>? userRole { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public string? LockoutEnabled { get; set; }
        public string? AccessFailedCount { get; set; }

        public string? Password { get; set; }
        public string? Email { get; set; }

        public string? Role { get; set; } // This is for display purposes only

        // For assigning roles during user creation or editing
        public string? SelectedRole { get; set; }
        public SelectList? AvailableRoles { get; set; }
    }

    public class SearchUsersViewModel
    {
        public string? SearchTerm { get; set; }
        public List<UsersViewModel>? Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web_Application.Models
{//admin panel page
    public class UsersViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
        public string Email { get; set; }

        public string Role { get; set; } // This is for display purposes only

        // For assigning roles during user creation or editing
        public string SelectedRole { get; set; }
        public SelectList AvailableRoles { get; set; }
    }

    public class SearchUsersViewModel
    {
        public string SearchTerm { get; set; }
        public List<UsersViewModel> Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

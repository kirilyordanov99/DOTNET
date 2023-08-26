using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Web_Application.Models
{
    public class ResetPasswordModel
    {
        public InputModel Input { get; set; }

        public ResetPasswordModel()
        {
            Input = new InputModel();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }
    }
}

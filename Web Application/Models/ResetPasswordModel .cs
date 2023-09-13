using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Text.Encodings.Web;

namespace Web_Application.Models
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Handle the case where the user doesn't exist.
                // You might want to show a message or redirect to a different page.
                return RedirectToPage("/Error");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { code = token, email = Input.Email },
                protocol: Request.Scheme);

            // Send the password reset email
            await SendPasswordResetEmailAsync(Input.Email, resetLink);

            // Redirect to the "ForgotPasswordConfirmation" page
            return RedirectToPage("/ForgotPasswordConfirmation");
        }


        private async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(resetLink)}'>clicking here</a>.";

            using (var client = new SmtpClient())
            {
                var credentials = new NetworkCredential
                {
                    UserName = "kirylvt@gmail.com",
                    Password = "yayulugdniweujkv"
                };

                client.Credentials = credentials;
                client.Host = "your-smtp-host"; // e.g., smtp.example.com
                client.Port = 587; // Use the appropriate SMTP port
                client.EnableSsl = true; // Use SSL if required

                var message = new MailMessage
                {
                    From = new MailAddress("kirylvt@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(email);

                await client.SendMailAsync(message);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Web_Application.Models;


namespace Web_Application.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UserController(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "User",
                new { email = user.Email, code = code }, protocol: Request.Scheme);

            var resetLink = $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>";
            var message = $"Please reset your password by clicking here: {resetLink}";

            // Send the email
            await _emailSender.SendEmailAsync(email, "Reset Password", message);

            return RedirectToAction("ForgotPasswordConfirmation");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }

            return View("Error");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "User",
                    new { userId = user.Id, code = token },
                    protocol: Request.Scheme);

                var emailConfirmationLink = $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>";
                var emailConfirmationMessage = $"Please confirm your email by clicking this {emailConfirmationLink}.";

                // Send the email
                await _emailSender.SendEmailAsync(user.Email, "Confirm Your Email", emailConfirmationMessage);

                return RedirectToAction("ConfirmationEmailSent");
            }

            return View("Error");
        }

        public IActionResult ConfirmationEmailSent()
        {
            return View();
        }
    }
}

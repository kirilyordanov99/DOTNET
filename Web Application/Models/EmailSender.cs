using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Web_Application.Models;
//password reset
namespace Web_Application.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var apiKey = _options.SendGridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_options.FromEmail, _options.FromName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            await client.SendEmailAsync(msg);
        }
    }
}

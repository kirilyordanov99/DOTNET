using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Web_Application.Services
{
    public class SendGridExample
    {
        public static async Task Execute(string email, string subject, string plainTextContent, string htmlContent)
        {
            var apiKey = "wnIXX6W_Tj6V5sOZf8Z6_A";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("wbppcrdspprt@gmail.com", "webappcrudsupport");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}

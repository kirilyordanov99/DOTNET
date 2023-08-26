namespace Web_Application.Models
{    //password reset
    public class EmailSenderOptions
    {
        public required string FromEmail { get; set; }
        public required string FromName { get; set; }
        public required string SendGridApiKey { get; set; }
    }
}

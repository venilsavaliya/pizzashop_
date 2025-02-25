using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Task> SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        // ServicePointManager.CheckCertificateRevocationList = false;

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(MailboxAddress.Parse("venilsavaliya999@gmail.com"));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        using (var client = new SmtpClient())
        {
            client.Connect(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), false);
            client.Authenticate(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
            client.Send(emailMessage);
            client.Disconnect(true);
        }

        return Task.CompletedTask;
    }

    Task IEmailService.SendEmailAsync(string to, string subject, string body)
    {
        return SendEmailAsync(to, subject, body);
    }
}

using MailKit.Security;
using MimeKit;
using PowerOutageNotifier.Configuration;

namespace PowerOutageNotifier;

public class EmailService
{
    private readonly EmailConfiguration _configuration;
    public EmailService(EmailConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string to, string message)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_configuration.Name, _configuration.Address));
        mailMessage.To.Add(MailboxAddress.Parse(to));
        mailMessage.Subject = "BEZ STRUJE";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message
        };
        var body = bodyBuilder.ToMessageBody();

        mailMessage.Body = body;

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        await smtp.ConnectAsync(_configuration.Host, _configuration.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_configuration.Username, _configuration.Password);
        string response = await smtp.SendAsync(mailMessage);
        await smtp.DisconnectAsync(true);
    }
}

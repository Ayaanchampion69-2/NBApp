// Services/EmailSender.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var host = _config["Email:Host"];
            var port = int.Parse(_config["Email:Port"] ?? "25");
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var from = _config["Email:From"];
            var senderName = _config["Email:SenderName"] ?? "NBApp";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, from));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };

            using var smtp = new SmtpClient();

            var useSsl = !string.IsNullOrEmpty(username);

            if (useSsl)
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            else
                await smtp.ConnectAsync(host, port, SecureSocketOptions.None);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                await smtp.AuthenticateAsync(username, password);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email} with subject '{Subject}'", email, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }
}
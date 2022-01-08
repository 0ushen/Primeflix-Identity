using IdentityServer.Interfaces;
using IdentityServer.Models;
using IdentityServer.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityServer.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private SmtpClient? _client;
    public EmailService(IOptions<EmailOptions> emailOptions, IConfiguration config)
    {
        var test = config.GetSection(EmailOptions.SectionName);
        _emailOptions = emailOptions.Value;
    }

    ~EmailService()
    {
        _client?.Disconnect(true);
    }

    public async Task SendEmailAsync(EmailRequest request)
    {
        var client = await InitSmtpClient();

        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_emailOptions.Mail),
            Subject = request.Subject
        };
        email.To.Add(MailboxAddress.Parse(request.EmailAddress));
        var builder = new BodyBuilder { HtmlBody = request.Body };
        email.Body = builder.ToMessageBody();

        await client.SendAsync(email);
    }

    private async Task<SmtpClient> InitSmtpClient()
    {
        if (_client is not null)
            return _client;

        _client = new SmtpClient();
        await _client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, false);
        await _client.AuthenticateAsync(_emailOptions.Mail, _emailOptions.Password);

        return _client;
    }
}

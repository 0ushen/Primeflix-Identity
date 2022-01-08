using IdentityServer.Interfaces;
using IdentityServer.Models;

namespace IdentityServer.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendChangePasswordLinkAsync(string userEmail, string? url)
    {
        var request = new EmailRequest(userEmail, "Change your Primeflix account password")
        {
            Body = $"You can change your password at this url : {url}"
        };

        await _emailService.SendEmailAsync(request);
    }
}

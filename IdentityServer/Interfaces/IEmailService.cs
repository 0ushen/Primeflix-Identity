using IdentityServer.Models;

namespace IdentityServer.Interfaces;
public interface IEmailService
{
    Task SendEmailAsync(EmailRequest request);
}

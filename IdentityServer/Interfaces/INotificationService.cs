namespace IdentityServer.Interfaces;

public interface INotificationService
{
    Task SendChangePasswordLinkAsync(string userEmail, string? url);
}
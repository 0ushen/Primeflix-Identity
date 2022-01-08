namespace IdentityServer.Models;

public class EmailRequest
{
    public EmailRequest(string emailAddress, string subject)
    {
        EmailAddress = emailAddress;
        Subject = subject;
    }
    public string EmailAddress { get; }
    public string Subject { get; }
    public string? Body { get; set; }
}

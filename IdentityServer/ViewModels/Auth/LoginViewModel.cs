using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ViewModels.Auth;

public class LoginViewModel : IdentityViewModel
{
    [Required]
    public string Username { get; set; }
    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "Max length exceeded")]
    [Required]
    public string Password { get; set; }

    [DisplayName("Remember me")]
    public bool StaySignedIn { get; set; }

    public IEnumerable<AuthenticationScheme>? ExternalProviders { get; set; }
}
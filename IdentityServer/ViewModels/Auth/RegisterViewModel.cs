using System.ComponentModel.DataAnnotations;

namespace IdentityServer.ViewModels.Auth;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    public string ReturnUrl { get; set; }
    [Required]
    public bool AgreeTerms { get; set; }
}
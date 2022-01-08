using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ViewModels.Auth
{
    public class ResetPasswordViewModel : IdentityViewModel
    {
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(100, ErrorMessage = "Length exceeded")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password Confirmation")]
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(100, ErrorMessage = "Length exceeded")]
        [Compare("Password", ErrorMessage = "Password and Password confirmation should be the same")]
        public string PasswordConfirmation { get; set; }
        [HiddenInput]
        public string Token { get; set; }
        [HiddenInput]
        public string UserId { get; set; }
    }
}
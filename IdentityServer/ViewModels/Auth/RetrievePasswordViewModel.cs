using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ViewModels.Auth
{
    public class RetrievePasswordViewModel : IdentityViewModel
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Wrong email format")]
        [Required(ErrorMessage = "This field is required")]
        [DisplayName("Email")]
        [MaxLength(100, ErrorMessage = "Length exceeded")]
        public string? Email { get; init; }
    }
}
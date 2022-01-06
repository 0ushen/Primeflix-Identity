using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.ViewModels.Auth
{
    public class RetrievePasswordViewModel
    {
        public RetrievePasswordViewModel()
        {

        }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Wrong email format")]
        [Required(ErrorMessage = "This field is required")]
        [DisplayName("Email")]
        [MaxLength(100, ErrorMessage = "Length exceeded")]
        public string? Email { get; init; }

        public string? ReturnUrl { get; init; }
    }
}
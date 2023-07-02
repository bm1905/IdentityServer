using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.ForgotPassword
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}

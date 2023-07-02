using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.EmailConfirmation
{
    public class ResendInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string Button { get; set; }
    }
}

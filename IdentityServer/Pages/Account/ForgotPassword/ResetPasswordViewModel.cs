namespace IdentityServer.Pages.Account.ForgotPassword
{
    public class ResetPasswordViewModel
    {
        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string ResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

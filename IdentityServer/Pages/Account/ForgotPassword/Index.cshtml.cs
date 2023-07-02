using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Models.Exceptions;
using Emailer.Model;
using Emailer.Services;

namespace IdentityServer.Pages.Account.ForgotPassword
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public IndexModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public ForgotPasswordViewModel Input { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            Input = new ForgotPasswordViewModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            if (!ModelState.IsValid) return Page();

            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return Page();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string domain = $"{Request.Scheme}://{Request.Host}";

            string resetLink = Url.Page("/Account/ForgotPassword/ResetPassword", new { token, email = user.Email });

            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(resetLink))
            {
                throw new InternalServerErrorException("Unable to generate password reset link");
            }

            string messageBody =
                $"Please use the reset link to reset the password.\n\r{domain}{resetLink}";
            Email message = new("MailBox", new[] { user.Email }, "Password reset email link", messageBody, null);
            await _emailService.SendEmailAsync(message);

            return RedirectToPage("/Account/ForgotPassword/ResetLinkSent", new { returnUrl });
        }
    }
}

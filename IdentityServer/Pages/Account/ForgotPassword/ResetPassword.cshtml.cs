using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account.ForgotPassword
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ResetPasswordViewModel Input { get; set; }

        public Task<IActionResult> OnGet(string token, string email)
        {
            Input = new ResetPasswordViewModel
            {
                Email = email,
                ResetToken = token
            };

            return Task.FromResult<IActionResult>(Page());
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            ApplicationUser user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("/Account/ForgotPassword/ConfirmReset", new { isSuccess = false } );
            }

            if (!string.Equals(Input.Password, Input.ConfirmPassword))
            {
                return Page();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, Input.ResetToken, Input.Password);
            return RedirectToPage("/Account/ForgotPassword/ConfirmReset", result.Succeeded ? new { isSuccess = true } : new { isSuccess = false });
        }
    }
}

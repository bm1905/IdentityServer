using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account.EmailConfirmation
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ConfirmEmailViewModel Input { get; set; }

        public async Task<IActionResult> OnGet(string token, string email)
        {
            Input = new ConfirmEmailViewModel();

            ApplicationUser user = await _userManager.FindByEmailAsync(email);


            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Input.ValidationSuccessful = true;
            }

            return Page();
        }
    }
}
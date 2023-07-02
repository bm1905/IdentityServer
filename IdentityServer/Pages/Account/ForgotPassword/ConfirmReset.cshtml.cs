using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account.ForgotPassword
{
    public class ConfirmResetModel : PageModel
    {
        [BindProperty] 
        public ConfirmResetViewModel Input { get; set; }

        public Task<IActionResult> OnGet(bool isSuccess)
        {
            Input = new ConfirmResetViewModel
            {
                ResetSuccessful = isSuccess
            };

            return Task.FromResult<IActionResult>(Page());
        }
    }
}

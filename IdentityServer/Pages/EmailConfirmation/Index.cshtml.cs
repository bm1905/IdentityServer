using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.EmailConfirmation
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public EmailConfirmationViewModel Input { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            Input = new EmailConfirmationViewModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }
    }
}

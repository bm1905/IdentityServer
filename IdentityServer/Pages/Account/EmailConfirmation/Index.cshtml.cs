using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account.EmailConfirmation
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ConfirmEmailViewModel Input { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            Input = new ConfirmEmailViewModel
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }
    }
}

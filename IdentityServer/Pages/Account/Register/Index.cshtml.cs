using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Emailer.Model;
using Emailer.Services;
using Shared.Models.Exceptions;

namespace IdentityServer.Pages.Account.Register;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailService _emailService;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleInManager,
        IEmailService emailService
    )
    {
        _roleManager = roleInManager;
        _userManager = userManager;
        _emailService = emailService;
    }


    [BindProperty]
    public RegisterViewModel Input { get; set; }

    public IActionResult OnGet(string returnUrl)
    {
        List<string> roles = new()
        {
            "admin",
            "regular"
        };
        ViewData["roles_message"] = roles;
        Input = new RegisterViewModel
        {
            ReturnUrl = returnUrl
        };
        return Page();
    }

    public async Task<IActionResult> OnPost(string returnUrl)
    {
        if (!ModelState.IsValid) return Page();

        ApplicationUser user = new()
        {
            UserName = Input.UserName,
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            EmailConfirmed = false
        };

        if (!string.Equals(Input.Password, Input.ConfirmPassword))
        {
            return Page();
        }

        IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
            if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
            {
                IdentityRole userRole = new()
                {
                    Name = Input.RoleName,
                    NormalizedName = Input.RoleName,

                };
                await _roleManager.CreateAsync(userRole);
            }
            await _userManager.AddToRoleAsync(user, Input.RoleName);


            await _userManager.AddClaimsAsync(user, new Claim[] {
                new(JwtClaimTypes.Name,Input.Email),
                new(JwtClaimTypes.Email,Input.Email),
                new(JwtClaimTypes.Role,Input.RoleName)
            });

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string domain = $"{Request.Scheme}://{Request.Host}";

            string confirmationLink = Url.Page("/Account/EmailConfirmation/ConfirmEmail", new { token, email = user.Email });
            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(confirmationLink))
            {
                throw new InternalServerErrorException("Unable to generate confirmation link");
            }

            string messageBody =
                $"Thank you for registering. Please use the confirmation link to confirm this email.\n\r{domain}{confirmationLink}";
            Email message = new("MailBox", new[] { user.Email }, "Confirmation email link", messageBody, null);
            await _emailService.SendEmailAsync(message);

            return RedirectToPage("/EmailConfirmation/Index", new { returnUrl });
        }

        ModelState.AddModelError("Errors", $"Registration unsuccessful: {string.Join(",", result.Errors.Select(p => p.Description))}");
        return Page();
    }
}
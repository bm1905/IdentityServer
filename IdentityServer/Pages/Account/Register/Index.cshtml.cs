using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Emailer.Services;

namespace IdentityServer.Pages.Account.Register;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
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
        _signInManager = signInManager;
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
        if (ModelState.IsValid)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = Input.UserName,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                EmailConfirmed = true // Change it to false when implementing email confirmation
            };

            if (!string.Equals(Input.Password, Input.ConfirmPassword))
            {
                return Page();
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync(Input.RoleName).GetAwaiter().GetResult())
                {
                    var userRole = new IdentityRole
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

                // Replace this with email confirmation
                var loginResult = await _signInManager.PasswordSignInAsync(
                    Input.UserName, Input.Password, false, lockoutOnFailure: true);

                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(Input.ReturnUrl))
                    {
                        return Redirect(Input.ReturnUrl);
                    }

                    if (string.IsNullOrEmpty(Input.ReturnUrl))
                    {
                        return Redirect("~/");
                    }

                    throw new Exception("invalid return URL");
                }

                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var confirmationLink = Url.Page("/EmailConfirmation/ConfirmEmail", new { token, email = user.Email });
                //if (confirmationLink == null)
                //{
                //    throw new Exception("Unable to generate confirmation link");
                //}
                //var message = new Email("MailBox", new string[] { user.Email }, "Confirmation email link", confirmationLink, null);
                //await _emailService.SendEmailAsync(message);

                //return RedirectToPage("/EmailConfirmation/Index", new { returnUrl });
            }

            ModelState.AddModelError("Errors", $"Registration unsuccessful: {string.Join(",", result.Errors.Select(p => p.Description))}");
        }
        return Page();
    }
}
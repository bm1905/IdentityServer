using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Pages.Account.Register;

public class RegisterViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; }
    public string RoleName { get; set; } = "regular";
}
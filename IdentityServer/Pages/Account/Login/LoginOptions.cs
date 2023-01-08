namespace IdentityServer.Pages.Account.Login;

public class LoginOptions
{
    public static bool AllowLocalLogin = true;
    public static bool AllowRememberLogin = true;
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
    public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    public static string EmailNotConfirmedErrorMessage = "Email not confirmed. Please check your email. Or click here to send another confirmation email";
}
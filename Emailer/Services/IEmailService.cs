using Emailer.Model;

namespace Emailer.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(Email email);
}
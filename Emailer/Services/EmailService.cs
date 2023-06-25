using Emailer.Config;
using Emailer.Model;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;
using Shared.Models.Exceptions;

namespace Emailer.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;

    public EmailService(EmailConfiguration emailConfig)
    {
        _emailConfig = emailConfig;
    }

    public async Task<bool> SendEmailAsync(Email email)
    {
        MimeMessage mailMessage = CreateEmailMessage(email);
        await SendAsync(mailMessage);
        return true;
    }

    private MimeMessage CreateEmailMessage(Email email)
    {
        MimeMessage emailMessage = new();
        emailMessage.From.Add(new MailboxAddress(_emailConfig.MailBoxName, _emailConfig.From));
        emailMessage.To.AddRange(email.To);
        emailMessage.Subject = email.Subject;

        BodyBuilder bodyBuilder = new() { HtmlBody = $"<p style='color:red;'>{email.Content}</p>" };

        if (email.Attachments != null && email.Attachments.Any())
        {
            foreach (IFormFile attachment in email.Attachments)
            {
                byte[] fileBytes;
                using (MemoryStream ms = new())
                {
                    attachment.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();
        return emailMessage;
    }

    private async Task SendAsync(MimeMessage mailMessage)
    {
        using SmtpClient client = new();
        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await client.SendAsync(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InternalServerErrorException($"Unable to send email with error: {ex.Message}", ex);
        }
        finally
        {
            await client.DisconnectAsync(true);
            client.Dispose();
        }
    }
}
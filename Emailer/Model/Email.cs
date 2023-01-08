using Microsoft.AspNetCore.Http;
using MimeKit;

namespace Emailer.Model;

public class Email
{
    public string MailBoxName { get; set; }
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public IFormFileCollection? Attachments { get; set; }

    public Email(string mailBoxName, IEnumerable<string> to, string subject, string content, IFormFileCollection? attachments)
    {
        MailBoxName = mailBoxName;
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress(mailBoxName, x)));
        Subject = subject;
        Content = content;
        Attachments = attachments;
    }
}
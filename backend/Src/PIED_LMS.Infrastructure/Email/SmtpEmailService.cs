using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using PIED_LMS.Contract.Abstractions.Email;

namespace PIED_LMS.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var settings = _configuration.GetSection("EmailSettings");
        using var client = new SmtpClient(settings["Host"], int.Parse(settings["Port"]))
        {
            Credentials = new NetworkCredential(settings["SenderEmail"], settings["SenderPassword"]),
            EnableSsl = true
        };
        
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(settings["SenderEmail"], "PIED LMS"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(to);
        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}

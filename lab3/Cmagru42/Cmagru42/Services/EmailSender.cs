using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Presentation.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new MailMessage()
            {
                From = new MailAddress("terman.emil@gmail.com"),
                Subject = subject,
                Body = message,
            };
            msg.To.Add(email);
            var smtpClient = new SmtpClient
            {
                Credentials = new NetworkCredential("aadiv.zakari@0hdear.com", "qazyc8Q$"),
                Host = "smtp.sendgrid.net",
                Port = 587
            };
            smtpClient.Send(msg);
            return Task.CompletedTask;
            //return client.SendEmailAsync(msg);
        }
    }
}

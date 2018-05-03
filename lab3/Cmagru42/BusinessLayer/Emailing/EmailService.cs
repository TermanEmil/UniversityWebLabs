using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MimeKit;
using MimeKit.Text;

namespace BusinessLayer.Emailing
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            throw new NotImplementedException();
        }

        public async Task Send(EmailMessage emailMessage)
        {
            var smtpClient = new SmtpClient
            {
                Host = _emailConfiguration.SmtpServer,
                Port = _emailConfiguration.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _emailConfiguration.SmtpUsername,
                    _emailConfiguration.SmtpPassword)
            };
            var mailMsg = new MailMessage(
                _emailConfiguration.SmtpUsername,
                emailMessage.ToAddress,
                emailMessage.Subject,
                emailMessage.Content
            );
            mailMsg.IsBodyHtml = emailMessage.IsHtml;
            smtpClient.SendAsync(mailMsg, mailMsg);

            await Task.FromResult(0);
        }
    }
}

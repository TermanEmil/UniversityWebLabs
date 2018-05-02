using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Emailing
{
    public interface IEmailService
    {
        Task Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}

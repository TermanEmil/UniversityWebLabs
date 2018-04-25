using System;
using System.Threading.Tasks;

namespace Presentation.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}

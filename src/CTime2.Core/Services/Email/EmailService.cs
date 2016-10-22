using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using UwCore.Common;

namespace CTime2.Core.Services.Email
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(EmailMessage email)
        {
            Guard.NotNull(email, nameof(email));

            return EmailManager.ShowComposeNewEmailAsync(email).AsTask();
        }
    }
}
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

namespace CTime2.Core.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage email);
    }
}
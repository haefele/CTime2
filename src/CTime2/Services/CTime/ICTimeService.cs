using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http.Filters;

namespace CTime2.Services.CTime
{
    public interface ICTimeService
    {
        Task<User> Login(string companyId, string emailAddress, string password);
    }
}
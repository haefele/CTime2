using System.Threading.Tasks;

namespace CTime2.Core.Services.Phone
{
    public interface IPhoneService
    {
        bool CanCall { get; }

        void Call(string phoneNumber, string displayName);
    }
}
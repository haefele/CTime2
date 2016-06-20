using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Biometrics
{
    public interface IBiometricsService
    {
        Task RememberUserForBiometricAuthAsync(User user);

        bool HasRememberedUser();
        Task<User> BiometricAuthAsync();
    }
}
using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Biometrics
{
    public interface IBiometricsService
    {
        Task<bool> BiometricAuthDeviceIsAvailableAsync();
        Task<bool> HasUserForBiometricAuthAsync();

        Task RememberUserForBiometricAuthAsync(User user);
        Task<User> BiometricAuthAsync();
    }
}
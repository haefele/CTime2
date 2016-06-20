using System;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using CTime2.Core.Data;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.Biometrics
{
    public class BiometricsService : IBiometricsService
    {
        private readonly IApplicationStateService _applicationStateService;

        public BiometricsService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;
        }

        public async Task RememberUserForBiometricAuthAsync(User user)
        {
            Guard.NotNull(user, nameof(user));

            var result = await UserConsentVerifier.RequestVerificationAsync("Login merken.");

            if (result == UserConsentVerificationResult.Verified)
            {
                this._applicationStateService.Set("BiometricAuth", user, UwCore.Services.ApplicationState.ApplicationState.Roaming);
            }
        }

        public bool HasRememberedUser()
        {
            var user = this._applicationStateService.Get<User>("BiometricAuth", UwCore.Services.ApplicationState.ApplicationState.Roaming);
            return user != null;
        }

        public async Task<User> BiometricAuthAsync()
        {
            if (this.HasRememberedUser() == false)
                return null;

            var result = await UserConsentVerifier.RequestVerificationAsync("Login merken.");

            if (result != UserConsentVerificationResult.Verified)
                return null;

            return this._applicationStateService.Get<User>("BiometricAuth", UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }
    }
}

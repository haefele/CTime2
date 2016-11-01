using System;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Strings;
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


        public Task<bool> HasUserForBiometricAuthAsync()
        {
            return Task.FromResult(this._applicationStateService.GetBiometricAuthUser() != null);
        }
        public async Task<bool> BiometricAuthDeviceIsAvailableAsync()
        {
            var deviceStatus = await UserConsentVerifier.CheckAvailabilityAsync();
            return deviceStatus == UserConsentVerifierAvailability.Available;
        }

        public async Task RememberUserForBiometricAuthAsync(User user)
        {
            Guard.NotNull(user, nameof(user));

            var result = await UserConsentVerifier.RequestVerificationAsync(CTime2CoreResources.Get("BiometricsService.RememberLogin"));

            if (result == UserConsentVerificationResult.Verified)
            {
                this._applicationStateService.SetBiometricAuthUser(user);
            }
            else if (result == UserConsentVerificationResult.DeviceNotPresent)
            {
                throw new CTimeException(CTime2CoreResources.Get("BiometricsService.NoBiometricDevicePresent"));
            }
        }
        public async Task<User> BiometricAuthAsync()
        {
            if (await this.HasUserForBiometricAuthAsync() == false)
                return null;

            if (await this.BiometricAuthDeviceIsAvailableAsync() == false)
                return null;

            var result = await UserConsentVerifier.RequestVerificationAsync(CTime2CoreResources.Get("BiometricsService.Login"));

            if (result != UserConsentVerificationResult.Verified)
                return null;

            return this._applicationStateService.GetBiometricAuthUser();
        }
    }
}

using CTime2.Core.Data;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.ApplicationState
{
    public static class ApplicationStateServiceExtensions
    {
        public static User GetCurrentUser(this IApplicationStateService self)
        {
            return self.Get<User>("User", UwCore.Services.ApplicationState.ApplicationState.Local);
        }

        public static void SetCurrentUser(this IApplicationStateService self, User currentUser)
        {
            self.Set("User", currentUser, UwCore.Services.ApplicationState.ApplicationState.Local);
        }

        public static User GetBiometricAuthUser(this IApplicationStateService self)
        {
            return self.Get<User>("BiometricAuth", UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }

        public static void SetBiometricAuthUser(this IApplicationStateService self, User biometricAuthUser)
        {
            self.Set("BiometricAuth", biometricAuthUser, UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }
    }
}
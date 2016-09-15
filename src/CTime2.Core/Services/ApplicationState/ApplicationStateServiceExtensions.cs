using System;
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

        public static string GetAttendanceListImageCacheEtag(this IApplicationStateService self)
        {
            return self.Get<string>("AttendanceListImageCacheEtag", UwCore.Services.ApplicationState.ApplicationState.Local);
        }

        public static void SetAttendanceListImageCacheEtag(this IApplicationStateService self, string etag)
        {
            self.Set("AttendanceListImageCacheEtag", etag, UwCore.Services.ApplicationState.ApplicationState.Local);
        }

        public static TimeSpan GetWorkDayHours(this IApplicationStateService self)
        {
            return self.Get<TimeSpan?>("WorkDayHours", UwCore.Services.ApplicationState.ApplicationState.Roaming) ?? TimeSpan.FromHours(8);
        }

        public static void SetWorkDayHours(this IApplicationStateService self, TimeSpan hours)
        {
            self.Set("WorkDayHours", hours, UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }

        public static TimeSpan GetWorkDayBreak(this IApplicationStateService self)
        {
            return self.Get<TimeSpan?>("WorkDayBreak", UwCore.Services.ApplicationState.ApplicationState.Roaming) ?? TimeSpan.FromHours(1);
        }

        public static void SetWorkDayBreak(this IApplicationStateService self, TimeSpan hours)
        {
            self.Set("WorkDayBreak", hours, UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }
    }
}
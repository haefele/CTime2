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
    }
}
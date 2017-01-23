using CTime2.Core.Data;
using Microsoft.HockeyApp;

namespace CTime2.Extensions
{
    public static class HockeyClientExtensions
    {
        public static void UpdateContactInfo(this IHockeyClient self, User currentUser)
        {
            if (currentUser == null)
            {
                self.UpdateContactInfo(null, null);
            }
            else
            {
                self.UpdateContactInfo($"{currentUser.FirstName} {currentUser.Name}", currentUser.Email);
            }
        }
    }
}
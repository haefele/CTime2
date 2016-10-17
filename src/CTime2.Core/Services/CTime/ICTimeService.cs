using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.CTime
{
    public interface ICTimeService
    {
        Task<User> Login(string emailAddress, string password);
        Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end);
        Task SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state, bool withGeolocation);
        Task<Time> GetCurrentTime(string employeeGuid);
        Task<IList<AttendingUser>> GetAttendingUsers(string companyId, byte[] defaultImage);
    }

    public static class CTimeServiceExtensions
    {
        public static async Task SaveTimer(this ICTimeService self, User user, TimeState state)
        {
            await self.SaveTimer(user.Id, DateTime.Now, user.CompanyId, state, user.SupportsGeoLocation);
        }

        public static async Task<bool> IsCurrentlyCheckedIn(this ICTimeService self, string employeeGuid)
        {
            var currentTime = await self.GetCurrentTime(employeeGuid);
            return currentTime != null && currentTime.State.IsEntered();
        }
    }
}
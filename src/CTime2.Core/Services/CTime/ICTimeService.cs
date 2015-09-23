using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.CTime
{
    public interface ICTimeService
    {
        Task<User> Login(string companyId, string emailAddress, string password);
        Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end);
        Task<bool> SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state);
        Task<Time> GetCurrentTime(string employeeGuid);
        Task<IList<AttendingUser>> GetAttendingUsers(string companyId);
    }
}
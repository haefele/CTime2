using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http.Filters;

namespace CTime2.Services.CTime
{
    public interface ICTimeService
    {
        Task<User> Login(string companyId, string emailAddress, string password);
        Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end);
        Task<bool> SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state);
    }
}
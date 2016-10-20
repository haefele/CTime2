using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CTime2.Core.Services.EmployeeGroups.Data;

namespace CTime2.Core.Services.EmployeeGroups
{
    public interface IEmployeeGroupService
    {
        Task<EmployeeGroup> GetGroupAsync(string userId, string groupId);
        Task<IList<EmployeeGroup>> GetGroupsAsync(string userId);
        Task<EmployeeGroup> CreateGroupAsync(string userId, string name, IList<string> employeeIds);
        Task DeleteGroupAsync(string userId, string employeeGroupId);
    }
}
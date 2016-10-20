using CTime2.Core.Services.EmployeeGroups;
using CTime2.Core.Services.EmployeeGroups.Data;

namespace CTime2.Core.Events
{
    public class EmployeeGroupCreated
    {
        public EmployeeGroup EmployeeGroup { get; }

        public EmployeeGroupCreated(EmployeeGroup employeeGroup)
        {
            this.EmployeeGroup = employeeGroup;
        }
    }
}
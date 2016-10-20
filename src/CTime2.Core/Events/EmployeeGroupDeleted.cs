using CTime2.Core.Services.EmployeeGroups;
using CTime2.Core.Services.EmployeeGroups.Data;

namespace CTime2.Core.Events
{
    public class EmployeeGroupDeleted
    {
        public EmployeeGroup EmployeeGroup { get; }

        public EmployeeGroupDeleted(EmployeeGroup employeeGroup)
        {
            this.EmployeeGroup = employeeGroup;
        }
    }
}
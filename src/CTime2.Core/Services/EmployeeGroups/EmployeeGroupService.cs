using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Events;
using CTime2.Core.Services.EmployeeGroups.Data;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.EmployeeGroups
{
    public class EmployeeGroupService : IEmployeeGroupService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationStateService _applicationStateService;

        public EmployeeGroupService(IEventAggregator eventAggregator, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._eventAggregator = eventAggregator;
            this._applicationStateService = applicationStateService;
        }

        public async Task<EmployeeGroup> GetGroupAsync(string userId, string groupId)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));
            Guard.NotNullOrWhiteSpace(groupId, nameof(groupId));

            var groups = this.GetEmployeeGroupsForUser(userId);
            return groups.FirstOrDefault(f => f.Id == groupId);
        }

        public async Task<IList<EmployeeGroup>> GetGroupsAsync(string userId)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));

            return this.GetEmployeeGroupsForUser(userId);
        }

        public async Task<EmployeeGroup> CreateGroupAsync(string userId, string name, IList<string> employeeIds)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNullOrEmpty(employeeIds, nameof(employeeIds));

            var group = new EmployeeGroup
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                EmployeeIds = employeeIds,
            };

            var groups = this.GetEmployeeGroupsForUser(userId);
            groups.Add(group);
            this.SetEmployeeGroupsForUser(userId, groups);

            this._eventAggregator.PublishOnUIThread(new EmployeeGroupCreated(group));

            return group;
        }

        public async Task DeleteGroupAsync(string userId, string employeeGroupId)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));
            Guard.NotNullOrWhiteSpace(employeeGroupId, nameof(employeeGroupId));

            var groups = this.GetEmployeeGroupsForUser(userId);

            var groupToRemove = groups.FirstOrDefault(f => f.Id == employeeGroupId);

            if (groupToRemove == null)
                return;

            groups.Remove(groupToRemove);
            this.SetEmployeeGroupsForUser(userId, groups);

            this._eventAggregator.PublishOnUIThread(new EmployeeGroupDeleted(groupToRemove));
        }


        private IList<EmployeeGroup> GetEmployeeGroupsForUser(string userId)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));

            var all = this.GetUserIdToEmployeeGroups();
            return all.GetValueOrDefault(userId) ?? new List<EmployeeGroup>();
        }

        private void SetEmployeeGroupsForUser(string userId, IList<EmployeeGroup> employeeGroups)
        {
            Guard.NotNullOrWhiteSpace(userId, nameof(userId));
            Guard.NotNull(employeeGroups, nameof(employeeGroups));

            var all = this.GetUserIdToEmployeeGroups();

            all[userId] = employeeGroups;

            this.SetUserIdToEmployeeGroups(all);
        }

        private Dictionary<string, IList<EmployeeGroup>> GetUserIdToEmployeeGroups()
        {
            var userIdToEmployeeGroups = this._applicationStateService.Get<Dictionary<string, IList<EmployeeGroup>>>("EmployeeGroups", UwCore.Services.ApplicationState.ApplicationState.Roaming);
            return userIdToEmployeeGroups ?? new Dictionary<string, IList<EmployeeGroup>>();
        }

        private void SetUserIdToEmployeeGroups(Dictionary<string, IList<EmployeeGroup>> userIdToEmployeeGroups)
        {
            Guard.NotNull(userIdToEmployeeGroups, nameof(userIdToEmployeeGroups));

            this._applicationStateService.Set("EmployeeGroups", userIdToEmployeeGroups, UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }
    }
}
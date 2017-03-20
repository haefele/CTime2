using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using AS = UwCore.Services.ApplicationState.ApplicationState;

namespace CTime2.Core.Services.EmployeeNotification
{
    public class EmployeeNotificationService : IEmployeeNotificationService
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ReaderWriterLockSlim _lock;

        public EmployeeNotificationService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService.GetStateServiceFor(typeof(EmployeeNotificationService));
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public bool ShouldNotify(string attendingUserId)
        {
            try
            {
                this._lock.EnterReadLock();

                var users = this.GetUsers();
                return users.Contains(attendingUserId);
            }
            finally
            {
                this._lock.ExitReadLock();
            }
        }

        public void Add(string attendingUserId)
        {
            try
            {
                this._lock.EnterWriteLock();

                var users = this.GetUsers().ToList();

                if (users.Contains(attendingUserId) == false)
                    users.Add(attendingUserId);

                this.SetUsers(users.ToArray());
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public void Remove(string attendingUserId)
        {
            try
            {
                this._lock.EnterWriteLock();

                var users = this.GetUsers().ToList();

                users.Remove(attendingUserId);

                this.SetUsers(users.ToArray());
            }
            finally
            {
                this._lock.ExitWriteLock();
            }
        }

        public Task SendNotificationsAsync()
        {
            throw new System.NotImplementedException();
        }

        #region Private Methods
        private string[] GetUsers()
        {
            return this._applicationStateService.Get<string[]>("Users", AS.Roaming) ?? new string[0];
        }
        private void SetUsers(string[] users)
        {
            this._applicationStateService.Set("Users", users, AS.Roaming);
        }
        #endregion
    }
}
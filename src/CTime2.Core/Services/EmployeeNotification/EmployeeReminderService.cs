using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Strings;
using Microsoft.Toolkit.Uwp.Notifications;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using AS = UwCore.Services.ApplicationState.ApplicationState;

namespace CTime2.Core.Services.EmployeeNotification
{
    public class EmployeeNotificationService : IEmployeeNotificationService
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IApplicationStateService _myApplicationStateService;
        private readonly ICTimeService _ctimeService;

        private readonly ReaderWriterLockSlim _lock;

        public EmployeeNotificationService(IApplicationStateService applicationStateService, ICTimeService ctimeService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(ctimeService, nameof(ctimeService));

            this._applicationStateService = applicationStateService;
            this._myApplicationStateService = applicationStateService.GetStateServiceFor(typeof(EmployeeNotificationService));
            this._ctimeService = ctimeService;

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

        public async Task SendNotificationsAsync()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();

            if (currentUser == null)
                return;

            var usersToCheck = this.GetUsers().ToList();
            var attendingUsers = await this._ctimeService.GetAttendingUsers(currentUser.CompanyId, new byte[0]);

            foreach (var user in new List<string>(usersToCheck))
            {
                var attendingUser = attendingUsers.FirstOrDefault(f => f.Id == user);

                if (attendingUser == null)
                    continue;

                if (attendingUser.AttendanceState.IsAttending)
                {
                    usersToCheck.Remove(user);

                    var toastContent = this.GetToastFor(attendingUser);
                    var toast = new ToastNotification(toastContent.GetXml());

                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }

            this.SetUsers(usersToCheck.ToArray());
        }

        #region Private Methods
        private ToastContent GetToastFor(AttendingUser user)
        {
            return new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = $"{user.FirstName} {user.Name}"
                            },
                            new AdaptiveText
                            {
                                Text = CTime2CoreResources.Get("Notification.EmployeeJustCheckedIn"),
                            },
                        }
                    }
                }
            };
        }

        private string[] GetUsers()
        {
            return this._myApplicationStateService.Get<string[]>("Users", AS.Roaming) ?? new string[0];
        }
        private void SetUsers(string[] users)
        {
            this._myApplicationStateService.Set("Users", users, AS.Roaming);
        }
        #endregion
    }
}
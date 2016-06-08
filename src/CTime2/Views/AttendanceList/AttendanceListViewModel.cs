using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.AttendanceList
{
    public class AttendanceListViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<AttendingUserByIsAttending>> _usersHelper;

        public ReactiveObservableCollection<AttendingUserByIsAttending> Users => this._usersHelper.Value;

        public ReactiveCommand<ReactiveObservableCollection<AttendingUserByIsAttending>> LoadUsers { get; }

        public AttendanceListViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            this.DisplayName = CTime2Resources.Get("Navigation.AttendanceList");
            
            this.LoadUsers = ReactiveCommand.CreateAsyncTask(_ => this.LoadUsersImpl());
            this.LoadUsers.AttachLoadingService(CTime2Resources.Get("Loading.AttendanceList"));
            this.LoadUsers.AttachExceptionHandler();
            this.LoadUsers.ToProperty(this, f => f.Users, out this._usersHelper);
        }

        private async Task<ReactiveObservableCollection<AttendingUserByIsAttending>> LoadUsersImpl()
        {
            var attendingUsers = await this._cTimeService.GetAttendingUsers(this._applicationStateService.GetCurrentUser().CompanyId);
            return new ReactiveObservableCollection<AttendingUserByIsAttending>(AttendingUserByIsAttending.Create(attendingUsers));
        }

        protected override async void OnActivate()
        {
            await this.LoadUsers.ExecuteAsyncTask();
        }
    }
}
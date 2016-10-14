using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Navigation;

namespace CTime2.Views.AttendanceList
{
    public class AttendanceListViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;

        private AttendingUser _selectedUser;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<AttendingUserByIsAttending>> _usersHelper;

        public AttendingUser SelectedUser
        {
            get { return this._selectedUser; }
            set { this.RaiseAndSetIfChanged(ref this._selectedUser, value); }
        }
        public ReactiveObservableCollection<AttendingUserByIsAttending> Users => this._usersHelper.Value;
        
        public UwCoreCommand<ReactiveObservableCollection<AttendingUserByIsAttending>> LoadUsers { get; }
        public UwCoreCommand<Unit> ShowDetails { get; }

        public AttendanceListViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, INavigationService navigationService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;

            this.DisplayName = CTime2Resources.Get("Navigation.AttendanceList");

            this.LoadUsers = UwCoreCommand.Create(this.LoadUsersImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.AttendanceList"))
                .HandleExceptions()
                .TrackEvent("LoadAttendanceList");

            var canShowDetails = this
                .WhenAnyValue(f => f.SelectedUser)
                .Select(f => f != null);

            this.ShowDetails = UwCoreCommand.Create(canShowDetails, this.ShowDetailsImpl)
                .HandleExceptions()
                .TrackEvent("ShowAttendingUserDetails");

            this.LoadUsers.ToProperty(this, f => f.Users, out this._usersHelper);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadUsers.ExecuteAsync();
        }

        private async Task<ReactiveObservableCollection<AttendingUserByIsAttending>> LoadUsersImpl()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();

            var attendingUsers = await this._cTimeService.GetAttendingUsers(currentUser.CompanyId, currentUser.CompanyImageAsPng);
            return new ReactiveObservableCollection<AttendingUserByIsAttending>(AttendingUserByIsAttending.Create(attendingUsers));
        }

        private Task ShowDetailsImpl()
        {
            this._navigationService.Popup
                .For<AttendingUserDetailsViewModel>()
                .WithParam(f => f.AttendingUserId, this.SelectedUser.Id)
                .Navigate();

            return Task.CompletedTask;
        }
    }
}
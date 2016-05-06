using Caliburn.Micro;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using UwCore.Services.ApplicationState;
using UwCore.Services.Loading;

namespace CTime2.Views.AttendanceList
{
    public class AttendanceListViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _sessionStateService;
        private readonly ILoadingService _loadingService;

        public BindableCollection<AttendingUserByIsAttending> Users { get; } 

        public AttendanceListViewModel(ICTimeService cTimeService, IApplicationStateService sessionStateService, ILoadingService loadingService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;

            this.DisplayName = CTime2Resources.Get("Navigation.AttendanceList");

            this.Users = new BindableCollection<AttendingUserByIsAttending>();
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.AttendanceList")))
            {
                var attendingUsers = await this._cTimeService.GetAttendingUsers(this._sessionStateService.GetCompanyId());

                this.Users.Clear();
                this.Users.AddRange(AttendingUserByIsAttending.Create(attendingUsers));
            }
        }
    }
}
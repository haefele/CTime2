using Caliburn.Micro;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Services.Loading;

namespace CTime2.Views.AttendanceList
{
    public class AttendanceListViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoadingService _loadingService;

        public BindableCollection<AttendingUserByIsAttending> Users { get; } 

        public AttendanceListViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;

            this.DisplayName = "Anwesenheitsliste";

            this.Users = new BindableCollection<AttendingUserByIsAttending>();
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show("Lade Anwesenheitsliste..."))
            {
                var attendingUsers = await this._cTimeService.GetAttendingUsers(this._sessionStateService.CompanyId);

                this.Users.Clear();
                this.Users.AddRange(AttendingUserByIsAttending.Create(attendingUsers));
            }
        }
    }
}
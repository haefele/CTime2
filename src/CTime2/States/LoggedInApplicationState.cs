using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Core.Services.ApplicationState;
using CTime2.Strings;
using CTime2.Views.AttendanceList;
using CTime2.Views.Overview;
using CTime2.Views.Settings;
using CTime2.Views.Statistics;
using CTime2.Views.YourTimes;
using UwCore.Application;
using UwCore.Common;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace CTime2.States
{
    public class LoggedInApplicationState : ApplicationMode
    {
        private readonly IApplicationStateService _applicationStateService;

        private readonly HamburgerItem _overviewHamburgerItem;
        private readonly HamburgerItem _myTimesHamburgerItem;
        private readonly HamburgerItem _attendanceListHamburgerItem;
        private readonly HamburgerItem _logoutHamburgerItem;
        private readonly HamburgerItem _statisticsItem;

        public LoggedInApplicationState(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;

            this._overviewHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Overview"), Symbol.Globe, typeof(OverviewViewModel));
            this._myTimesHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.MyTimes"), Symbol.Calendar, typeof(YourTimesViewModel));
            this._attendanceListHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.AttendanceList"), SymbolEx.AttendanceList, typeof(AttendanceListViewModel));
            this._logoutHamburgerItem = new ClickableHamburgerItem(CTime2Resources.Get("Navigation.Logout"), SymbolEx.Logout, this.Logout);
            this._statisticsItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Statistics"), SymbolEx.Statistics, typeof(StatisticsViewModel));
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._overviewHamburgerItem);
            this.Application.Actions.Add(this._myTimesHamburgerItem);
            this.Application.Actions.Add(this._attendanceListHamburgerItem);
            this.Application.SecondaryActions.Add(this._logoutHamburgerItem);
            this.Application.Actions.Add(this._statisticsItem);

            this._overviewHamburgerItem.Execute();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._overviewHamburgerItem);
            this.Application.Actions.Remove(this._myTimesHamburgerItem);
            this.Application.Actions.Remove(this._attendanceListHamburgerItem);
            this.Application.SecondaryActions.Remove(this._logoutHamburgerItem);
            this.Application.Actions.Remove(this._statisticsItem);
        }

        private async void Logout()
        {
            this._applicationStateService.SetCurrentUser(null);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationState>();
        }
    }
}
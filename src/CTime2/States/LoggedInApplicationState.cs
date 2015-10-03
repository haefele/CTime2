using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Core.Services.SessionState;
using CTime2.Services.Navigation;
using CTime2.Views.AttendanceList;
using CTime2.Views.Overview;
using CTime2.Views.Shell;
using CTime2.Views.StampTime;
using CTime2.Views.Statistics;
using CTime2.Views.YourTimes;

namespace CTime2.States
{
    public class LoggedInApplicationState : ApplicationState
    {
        private readonly ICTimeNavigationService _navigationService;
        private readonly ISessionStateService _sessionStateService;

        private readonly NavigationItemViewModel _overviewNavigationItem;
        private readonly NavigationItemViewModel _stampTimeNavigationItem;
        private readonly NavigationItemViewModel _myTimesNavigationItem;
        private readonly NavigationItemViewModel _attendanceListNavigationItem;
        private readonly NavigationItemViewModel _logoutNavigationItem;
        private readonly NavigationItemViewModel _statisticsItem;

        public LoggedInApplicationState(ICTimeNavigationService navigationService, ISessionStateService sessionStateService)
        {
            this._navigationService = navigationService;
            this._sessionStateService = sessionStateService;

            this._overviewNavigationItem = new NavigationItemViewModel(this.Overview, "Übersicht", Symbol.Globe);
            this._stampTimeNavigationItem = new NavigationItemViewModel(this.StampTime, "Stempeln", Symbol.Clock);
            this._myTimesNavigationItem = new NavigationItemViewModel(this.MyTimes, "Meine Zeiten", Symbol.Calendar);
            this._attendanceListNavigationItem = new NavigationItemViewModel(this.AttendanceList, "Anwesenheitsliste", SymbolEx.AttendanceList);
            this._logoutNavigationItem = new NavigationItemViewModel(this.Logout, "Abmelden", SymbolEx.Logout);
            this._statisticsItem = new NavigationItemViewModel(this.Statistics, "Statistiken", SymbolEx.Statistics);
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._overviewNavigationItem);
            this.Application.Actions.Add(this._stampTimeNavigationItem);
            this.Application.Actions.Add(this._myTimesNavigationItem);
            this.Application.Actions.Add(this._attendanceListNavigationItem);
            this.Application.SecondaryActions.Add(this._logoutNavigationItem);
            this.Application.Actions.Add(this._statisticsItem);

            this.Overview();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._overviewNavigationItem);
            this.Application.Actions.Remove(this._stampTimeNavigationItem);
            this.Application.Actions.Remove(this._myTimesNavigationItem);
            this.Application.Actions.Remove(this._attendanceListNavigationItem);
            this.Application.SecondaryActions.Remove(this._logoutNavigationItem);
            this.Application.Actions.Remove(this._statisticsItem);
        }

        private void Overview()
        {
            this._navigationService
                .For<OverviewViewModel>()
                .Navigate();
        }

        private void StampTime()
        {
            this._navigationService
                .For<StampTimeViewModel>()
                .Navigate();
        }

        private void MyTimes()
        {
            this._navigationService
                .For<YourTimesViewModel>()
                .Navigate();
        }

        private void AttendanceList()
        {
            this._navigationService
                .For<AttendanceListViewModel>()
                .Navigate();
        }

        private void Logout()
        {
            this._sessionStateService.CurrentUser = null;

            this.Application.CurrentState = IoC.Get<LoggedOutApplicationState>();
        }

        private void Statistics()
        {
            this._navigationService
                .For<StatisticsViewModel>()
                .Navigate();
        }
    }
}
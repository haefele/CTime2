using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Services.SessionState;
using CTime2.Views.Overview;

namespace CTime2.Views.Shell.States
{
    public class LoggedInShellState : ShellState
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionStateService _sessionStateService;

        private readonly NavigationItemViewModel _logoutNavigationItem;

        public LoggedInShellState(INavigationService navigationService, ISessionStateService sessionStateService)
        {
            this._navigationService = navigationService;
            this._sessionStateService = sessionStateService;

            this._logoutNavigationItem = new NavigationItemViewModel(this.Logout) { Label = "Logout", Symbol = Symbol.LeaveChat };
        }

        public override void Enter()
        {
            this.ViewModel.SecondaryActions.Add(this._logoutNavigationItem);

            this._navigationService
                .For<OverviewViewModel>()
                .Navigate();
        }

        public override void Leave()
        {
            this.ViewModel.SecondaryActions.Remove(this._logoutNavigationItem);
        }

        private void Logout()
        {
            this._sessionStateService.CurrentUser = null;
            this.ViewModel.CurrentState = IoC.Get<LoggedOutShellState>();
        }
    }
}
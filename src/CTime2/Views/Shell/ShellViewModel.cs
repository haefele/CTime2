using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Services.SessionState;
using CTime2.Views.Login;
using CTime2.Views.Shell.States;

namespace CTime2.Views.Shell
{
    public class ShellViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionStateService _sessionStateService;

        private ShellState _currentState;
        
        public BindableCollection<NavigationItemViewModel> Actions { get; }
        public BindableCollection<NavigationItemViewModel> SecondaryActions { get; }

        public ShellState CurrentState
        {
            get { return this._currentState; }
            set
            {
                this._currentState?.Leave();

                this._currentState = value;
                this._currentState.ViewModel = this;

                this._currentState?.Enter();

                this._navigationService.BackStack.Clear();
            }
        }

        public ShellViewModel(INavigationService navigationService, ISessionStateService sessionStateService)
        {
            this._navigationService = navigationService;
            this._sessionStateService = sessionStateService;

            this.Actions = new BindableCollection<NavigationItemViewModel>();
            this.SecondaryActions = new BindableCollection<NavigationItemViewModel>();
        }

        protected override void OnActivate()
        {
            if (this._sessionStateService.CurrentUser != null)
            {
                this.CurrentState = IoC.Get<LoggedInShellState>();
            }
            else
            {
                this.CurrentState = IoC.Get<LoggedOutShellState>();
            }
        }
    }
}
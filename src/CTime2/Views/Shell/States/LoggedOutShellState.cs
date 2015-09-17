using Caliburn.Micro;
using CTime2.Views.Login;

namespace CTime2.Views.Shell.States
{
    public class LoggedOutShellState : ShellState
    {
        private readonly INavigationService _navigationService;

        public LoggedOutShellState(INavigationService navigationService)
        {
            this._navigationService = navigationService;
        }

        public override void Enter()
        {
            this._navigationService
                .For<LoginViewModel>()
                .Navigate();
        }

        public override void Leave()
        {
        }
    }
}
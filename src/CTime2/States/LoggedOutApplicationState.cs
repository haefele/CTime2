using Caliburn.Micro;
using CTime2.Views.Login;

namespace CTime2.States
{
    public class LoggedOutApplicationState : ApplicationState
    {
        private readonly INavigationService _navigationService;

        public LoggedOutApplicationState(INavigationService navigationService)
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
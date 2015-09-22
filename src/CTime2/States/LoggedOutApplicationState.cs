using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Views.Login;
using CTime2.Views.Shell;

namespace CTime2.States
{
    public class LoggedOutApplicationState : ApplicationState
    {
        private readonly INavigationService _navigationService;

        private readonly NavigationItemViewModel _loginNavigationItem;

        public LoggedOutApplicationState(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this._loginNavigationItem = new NavigationItemViewModel(this.Login, "Anmelden", SymbolEx.Login);
        }

        private void Login()
        {
            this._navigationService
                .For<LoginViewModel>()
                .Navigate();
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._loginNavigationItem);

            this._navigationService
                .For<LoginViewModel>()
                .Navigate();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._loginNavigationItem);
        }
    }
}
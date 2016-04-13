using Caliburn.Micro;
using CTime2.Common;
using CTime2.Services.Navigation;
using CTime2.Strings;
using CTime2.Views.Login;
using CTime2.Views.Shell;

namespace CTime2.States
{
    public class LoggedOutApplicationState : ApplicationState
    {
        private readonly ICTimeNavigationService _navigationService;

        private readonly HamburgerItem _loginHamburgerItem;

        public LoggedOutApplicationState(ICTimeNavigationService navigationService)
        {
            this._navigationService = navigationService;

            this._loginHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Login"), SymbolEx.Login, typeof(LoginViewModel));
        }
        
        public override void Enter()
        {
            this.Application.Actions.Add(this._loginHamburgerItem);

            this._navigationService.Navigate(typeof(LoginViewModel));
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._loginHamburgerItem);
        }
    }
}
using CTime2.Common;
using CTime2.Strings;
using CTime2.Views.Login;
using UwCore.Application;
using UwCore.Hamburger;

namespace CTime2.ApplicationModes
{
    public class LoggedOutApplicationMode : ApplicationMode
    {
        private readonly HamburgerItem _loginHamburgerItem;

        public LoggedOutApplicationMode()
        {
            this._loginHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Login"), SymbolEx.Login, typeof(LoginViewModel));
        }
        
        public override void Enter()
        {
            this.Application.Actions.Add(this._loginHamburgerItem);

            this._loginHamburgerItem.Execute();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._loginHamburgerItem);
        }
    }
}
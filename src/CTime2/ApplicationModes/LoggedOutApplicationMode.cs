using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
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
        private readonly HamburgerItem _switchToTerminalHamburgerItem;

        public LoggedOutApplicationMode()
        {
            this._loginHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Login"), SymbolEx.Login, typeof(LoginViewModel));
            this._switchToTerminalHamburgerItem = new ClickableHamburgerItem("Zum Terminal", Symbol.AllApps, this.SwitchToTerminal);
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._loginHamburgerItem);
            this.Application.SecondaryActions.Insert(0, this._switchToTerminalHamburgerItem);

            this._loginHamburgerItem.Execute();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._loginHamburgerItem);
            this.Application.SecondaryActions.Remove(this._switchToTerminalHamburgerItem);
        }

        private void SwitchToTerminal()
        {
            this.Application.CurrentMode = IoC.Get<TerminalApplicationMode>();
        }
    }
}
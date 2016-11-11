using System.Threading.Tasks;
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
    public class LoggedOutApplicationMode : ShellMode
    {
        private readonly HamburgerItem _loginHamburgerItem;
        private readonly HamburgerItem _switchToTerminalHamburgerItem;

        public LoggedOutApplicationMode()
        {
            this._loginHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Login"), SymbolEx.Login, typeof(LoginViewModel));
            this._switchToTerminalHamburgerItem = new ClickableHamburgerItem("Zum Terminal", Symbol.AllApps, this.SwitchToTerminal);
        }

        protected override async Task OnEnter()
        {
            await base.OnEnter();

            this._loginHamburgerItem.Execute();
        }

        protected override async Task AddActions()
        {
            await base.AddActions();

            this.Shell.Actions.Add(this._loginHamburgerItem);
            this.Shell.SecondaryActions.Insert(0, this._switchToTerminalHamburgerItem);
        }

        protected override async Task RemoveActions()
        {
            await base.RemoveActions();

            this.Shell.Actions.Remove(this._loginHamburgerItem);
            this.Shell.SecondaryActions.Remove(this._switchToTerminalHamburgerItem);
        }
        
        private void SwitchToTerminal()
        {
            this.Shell.CurrentMode = IoC.Get<TerminalApplicationMode>();
        }
    }
}
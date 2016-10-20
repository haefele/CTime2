using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Views.Terminal;
using UwCore.Application;
using UwCore.Hamburger;

namespace CTime2.ApplicationModes
{
    public class TerminalApplicationMode : ApplicationMode
    {
        private readonly NavigatingHamburgerItem _terminalHamburgerItem;
        private readonly ClickableHamburgerItem _goBackHamburgerItem;

        public TerminalApplicationMode()
        {
            this._terminalHamburgerItem = new NavigatingHamburgerItem("Terminal", Symbol.AllApps, typeof(TerminalViewModel));
            this._goBackHamburgerItem = new ClickableHamburgerItem("Zur normalen Anmeldung", Symbol.Back, this.GoBack);
        }

        protected override async Task OnEnter()
        {
            await base.OnEnter();

            this._terminalHamburgerItem.Execute();

            var appView = ApplicationView.GetForCurrentView();
            appView.TryEnterFullScreenMode();
        }

        protected override async Task OnLeave()
        {
            await base.OnLeave();

            var appView = ApplicationView.GetForCurrentView();
            appView.ExitFullScreenMode();
        }

        protected override async Task AddActions()
        {
            await base.AddActions();

            this.Application.Actions.Add(this._terminalHamburgerItem);
            this.Application.SecondaryActions.Insert(0, this._goBackHamburgerItem);
        }

        protected override async Task RemoveActions()
        {
            await base.RemoveActions();

            this.Application.Actions.Remove(this._terminalHamburgerItem);
            this.Application.SecondaryActions.Remove(this._goBackHamburgerItem);
        }

        private void GoBack()
        {
            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }
    }
}
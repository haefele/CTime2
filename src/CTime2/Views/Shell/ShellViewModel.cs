using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Services.SessionState;
using CTime2.States;
using CTime2.Views.About;
using CTime2.Views.Login;

namespace CTime2.Views.Shell
{
    public class ShellViewModel : Screen, IApplication
    {
        private readonly INavigationService _navigationService;

        private ApplicationState _currentState;
        
        public BindableCollection<NavigationItemViewModel> Actions { get; }
        public BindableCollection<NavigationItemViewModel> SecondaryActions { get; }

        public ApplicationState CurrentState
        {
            get { return this._currentState; }
            set
            {
                this._currentState?.Leave();

                this._currentState = value;
                this._currentState.Application = this;

                this._currentState?.Enter();

                this._navigationService.BackStack.Clear();
            }
        }

        public ShellViewModel(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this.Actions = new BindableCollection<NavigationItemViewModel>();
            this.SecondaryActions = new BindableCollection<NavigationItemViewModel>
            {
                new NavigationItemViewModel(this.About, "Über", Mdl2.Help)
            };
        }

        private void About()
        {
            this._navigationService.For<AboutViewModel>().Navigate();
        }
    }
}
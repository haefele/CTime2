using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Core.Services.Band;
using CTime2.Services.Loading;
using CTime2.Services.Navigation;
using CTime2.States;
using CTime2.Strings;
using CTime2.Views.About;

namespace CTime2.Views.Shell
{
    public class ShellViewModel : Screen, IApplication
    {
        private readonly ICTimeNavigationService _navigationService;

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

                this._navigationService.ClearBackStack();
            }
        }

        public ShellViewModel(ICTimeNavigationService navigationService)
        {
            this._navigationService = navigationService;

            this.Actions = new BindableCollection<NavigationItemViewModel>();
            this.SecondaryActions = new BindableCollection<NavigationItemViewModel>
            {
                new NavigationItemViewModel(this.Test, "Test", Symbol.Admin),
                new NavigationItemViewModel(this.About, CTime2Resources.Get("Navigation.About"), SymbolEx.Help),
            };
        }

        private async void Test()
        {
            using (IoC.Get<ILoadingService>().Show("Band"))
            {
                var bandService = IoC.Get<IBandService>();
                await bandService.RegisterBandTileAsync();

                await bandService.ListenForEventsAsync(() =>
                {

                }, () =>
                {

                });
            }
        }

        private void About()
        {
            this._navigationService.For<AboutViewModel>().Navigate();
        }
    }
}
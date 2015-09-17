using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using CTime2.Services.CTime;
using CTime2.Services.SessionState;
using CTime2.Views.Login;
using CTime2.Views.Overview;
using CTime2.Views.Shell;
using CTime2.Views.Shell.States;
using CTime2.Views.YourTimes;

namespace CTime2
{
    sealed partial class App : CaliburnApplication
    {
        private WinRTContainer _container;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            this._container = new WinRTContainer();
            this._container.RegisterWinRTServices();

            //ViewModels
            this._container
                .Singleton<ShellViewModel>()
                .PerRequest<LoginViewModel>()
                .PerRequest<OverviewViewModel>()
                .PerRequest<YourTimesViewModel>();

            //ShellStates
            this._container
                .PerRequest<LoggedOutShellState>()
                .PerRequest<LoggedInShellState>();

            //Services
            this._container
                .Singleton<ICTimeService, CTimeService>()
                .Singleton<ISessionStateService, SessionStateService>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return this._container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;

            this.Initialize();

            var view = new ShellView();
            this._container.RegisterNavigationService(view.ContentFrame);

            var viewModel = IoC.Get<ShellViewModel>();
            ViewModelBinder.Bind(viewModel, view, null);

            ScreenExtensions.TryActivate(viewModel);

            Window.Current.Content = view;
            Window.Current.Activate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using CTime2.Events;
using CTime2.Services.CTime;
using CTime2.Services.Dialog;
using CTime2.Services.Loading;
using CTime2.Services.SessionState;
using CTime2.States;
using CTime2.Views.About;
using CTime2.Views.Login;
using CTime2.Views.Overview;
using CTime2.Views.Shell;
using CTime2.Views.StampTime;
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
                .PerRequest<ShellViewModel>()
                .PerRequest<LoginViewModel>()
                .PerRequest<OverviewViewModel>()
                .PerRequest<YourTimesViewModel>()
                .PerRequest<StampTimeViewModel>()
                .PerRequest<AboutViewModel>();

            //ShellStates
            this._container
                .PerRequest<LoggedOutApplicationState>()
                .PerRequest<LoggedInApplicationState>();

            //Services
            this._container
                .Singleton<ICTimeService, CTimeService>()
                .Singleton<ISessionStateService, SessionStateService>()
                .Singleton<IDialogService, DialogService>();
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

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;

            this.Initialize();
            
            var stateService = this._container.GetInstance<ISessionStateService>();
            await stateService.RestoreStateAsync();

            var view = new ShellView();
            this._container.RegisterNavigationService(view.ContentFrame);
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));
            
            var viewModel = IoC.Get<ShellViewModel>();
            this._container.Instance((IApplication)viewModel);

            ViewModelBinder.Bind(viewModel, view, null);
            ScreenExtensions.TryActivate(viewModel);

            this.TryRestore(stateService, viewModel);

            Window.Current.Content = view;
            Window.Current.Activate();
        }

        protected override async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            var stateService = this._container.GetInstance<ISessionStateService>();
            await stateService.SaveStateAsync();

            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationSuspendingEvent());

            deferral.Complete();
        }

        protected override void OnResuming(object sender, object e)
        {
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationResumedEvent());
        }

        private void TryRestore(ISessionStateService stateService, ShellViewModel viewModel)
        {
            if (stateService.CurrentUser != null)
            {
                viewModel.CurrentState = IoC.Get<LoggedInApplicationState>();
            }
            else
            {
                viewModel.CurrentState = IoC.Get<LoggedOutApplicationState>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Caliburn.Micro;
using CTime2.BandService;
using CTime2.Core.Logging;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.Licenses;
using CTime2.Core.Services.SessionState;
using CTime2.Events;
using CTime2.Services.Dialog;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;
using CTime2.Services.Navigation;
using CTime2.States;
using CTime2.Views.About;
using CTime2.Views.AttendanceList;
using CTime2.Views.Band;
using CTime2.Views.Licenses;
using CTime2.Views.Login;
using CTime2.Views.Overview;
using CTime2.Views.Shell;
using CTime2.Views.StampTime;
using CTime2.Views.Statistics;
using CTime2.Views.YourTimes;
using CTime2.VoiceCommandService;

namespace CTime2
{
    sealed partial class App
    {
        #region Logger
        private static readonly Logger _logger = LoggerFactory.GetLogger<App>();
        #endregion

        #region Fields
        private WinRTContainer _container;
        #endregion

        #region Constructors
        public App()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Configure
        protected override void Configure()
        {
            this.ConfigureContainer();
            this.ConfigureCaliburnMicro();
            this.ConfigureVoiceCommands();
            this.ConfigureWindowMinSize();
        }

        private void ConfigureContainer()
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
                .PerRequest<AboutViewModel>()
                .PerRequest<AttendanceListViewModel>()
                .PerRequest<StatisticsViewModel>()
                .PerRequest<LicensesListViewModel>()
                .PerRequest<LicenseViewModel>()
                .PerRequest<CheckedInViewModel>()
                .PerRequest<CheckedOutViewModel>()
                .PerRequest<HomeOfficeCheckedInViewModel>()
                .PerRequest<TripCheckedInViewModel>()
                .PerRequest<BandViewModel>();
            
            //ShellStates
            this._container
                .PerRequest<LoggedOutApplicationState>()
                .PerRequest<LoggedInApplicationState>();

            //Services
            this._container
                .Singleton<ICTimeService, CTimeService>()
                .Singleton<ISessionStateService, SessionStateService>()
                .Singleton<IDialogService, DialogService>()
                .Singleton<IExceptionHandler, ExceptionHandler>()
                .Singleton<ILicensesService, LicensesService>()
                .Singleton<IBandService, Core.Services.Band.BandService>();
        }

        private void ConfigureCaliburnMicro()
        {
            ViewModelBinder.ApplyConventionsByDefault = false;
        }
        
        private async void ConfigureVoiceCommands()
        {
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("CTime2VoiceCommands.xml", CreationCollisionOption.ReplaceExisting);

            using (var vcd = typeof(CTime2VoiceCommandService).GetTypeInfo().Assembly.GetManifestResourceStream("CTime2.VoiceCommandService.CTime2VoiceCommands.xml"))
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                await vcd.CopyToAsync(fileStream);
            }

            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
        }

        private void ConfigureWindowMinSize()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }
        #endregion

        #region IoC
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
        #endregion

        #region Lifecycle
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;

            this.Initialize();
            
            var stateService = this._container.GetInstance<ISessionStateService>();
            await stateService.RestoreStateAsync();

            var view = new ShellView();
            this._container.Instance((ICTimeNavigationService)new CTimeNavigationService(view.ContentFrame));
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));
            
            var viewModel = IoC.Get<ShellViewModel>();
            this._container.Instance((IApplication)viewModel);
            
            viewModel.CurrentState = stateService.CurrentUser != null
                ? (ApplicationState)IoC.Get<LoggedInApplicationState>()
                : IoC.Get<LoggedOutApplicationState>();

            ViewModelBinder.Bind(viewModel, view, null);
            ScreenExtensions.TryActivate(viewModel);

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

        protected override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error(e.Exception, () => "Unhandled exception occured.");
        }
        #endregion
    }
}

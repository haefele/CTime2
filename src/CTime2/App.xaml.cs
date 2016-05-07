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
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Core.Common;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.Licenses;
using CTime2.States;
using CTime2.Strings;
using CTime2.Views.AttendanceList;
using CTime2.Views.Login;
using CTime2.Views.Overview;
using CTime2.Views.Settings;
using CTime2.Views.Settings.About;
using CTime2.Views.Settings.Band;
using CTime2.Views.Settings.Licenses;
using CTime2.Views.Settings.Start;
using CTime2.Views.StampTime;
using CTime2.Views.StampTime.CheckedIn;
using CTime2.Views.StampTime.CheckedOut;
using CTime2.Views.StampTime.HomeOfficeCheckedIn;
using CTime2.Views.StampTime.TripCheckedIn;
using CTime2.Views.Statistics;
using CTime2.Views.YourTimes;
using CTime2.VoiceCommandService;
using UwCore.Application;
using UwCore.Extensions;
using UwCore.Hamburger;
using UwCore.Logging;
using UwCore.Services.ApplicationState;

namespace CTime2
{
    sealed partial class App
    {
        private static readonly Logger _logger = LoggerFactory.GetLogger<App>();

        #region Constructors
        public App()
        {
            this.InitializeComponent();
        }
        #endregion

        protected override void Configure()
        {
            base.Configure();

            this.ConfigureVoiceCommands();
            this.ConfigureWindowMinSize();
        }

        public override void CustomizeApplication(IApplication application)
        {
            application.SecondaryActions.Add(new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Settings"), Symbol.Setting, typeof(SettingsViewModel)));
        }

        public override void ConfigureContainer(WinRTContainer container)
        {
            container
                .PerRequest<LoggedOutApplicationState>()
                .PerRequest<LoggedInApplicationState>();

            container
                .Singleton<ICTimeService, CTimeService>()
                .Singleton<ILicensesService, LicensesService>()
                .Singleton<IBandService, Core.Services.Band.BandService>();
        }

        public override ApplicationMode GetCurrentMode()
        {
            return IoC.Get<IApplicationStateService>().GetCurrentUser() != null
                ? (ApplicationMode)IoC.Get<LoggedInApplicationState>()
                : IoC.Get<LoggedOutApplicationState>();
        }

        public override string GetErrorTitle()
        {
            return CTime2Resources.Get("ExceptionHandler.ErrorTitle");
        }

        public override string GetErrorMessage()
        {
            return CTime2Resources.Get("ExceptionHandler.ErrorMessage");
        }

        public override Type GetCommonExceptionType()
        {
            return typeof(CTimeException);
        }

        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(LoginViewModel);
            yield return typeof(OverviewViewModel);
            yield return typeof(YourTimesViewModel);
            yield return typeof(StampTimeViewModel);
            yield return typeof(AboutViewModel);
            yield return typeof(AttendanceListViewModel);
            yield return typeof(StatisticsViewModel);
            yield return typeof(LicensesListViewModel);
            yield return typeof(LicenseViewModel);
            yield return typeof(CheckedInViewModel);
            yield return typeof(CheckedOutViewModel);
            yield return typeof(HomeOfficeCheckedInViewModel);
            yield return typeof(TripCheckedInViewModel);
            yield return typeof(BandViewModel);
            yield return typeof(SettingsViewModel);
            yield return typeof(StartViewModel);
        }
        
        private async void ConfigureVoiceCommands()
        {
            try
            {
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("CTime2VoiceCommands.xml", CreationCollisionOption.ReplaceExisting);

                using (var vcd = typeof (CTime2VoiceCommandService).GetTypeInfo().Assembly.GetManifestResourceStream("CTime2.VoiceCommandService.CTime2VoiceCommands.xml"))
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    await vcd.CopyToAsync(fileStream);
                }

                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"For some reason the voice-command registration failed. {exception.GetFullMessage()}");
            }
        }

        private void ConfigureWindowMinSize()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.ApplicationModes;
using CTime2.Core.Common;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using CTime2.Core.Services.Contacts;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.CTime.RequestCache;
using CTime2.Core.Services.Email;
using CTime2.Core.Services.EmployeeGroups;
using CTime2.Core.Services.EmployeeNotification;
using CTime2.Core.Services.GeoLocation;
using CTime2.Core.Services.Licenses;
using CTime2.Core.Services.Phone;
using CTime2.Core.Services.Sharing;
using CTime2.Core.Services.Statistics;
using CTime2.Core.Services.Tile;
using CTime2.EmployeeNotificationService;
using CTime2.LiveTileService;
using CTime2.Strings;
using CTime2.Views.About;
using CTime2.Views.AttendanceList;
using CTime2.Views.GeoLocationInfo;
using CTime2.Views.HeaderDetails;
using CTime2.Views.Login;
using CTime2.Views.Overview;
using CTime2.Views.Overview.CheckedIn;
using CTime2.Views.Overview.CheckedOut;
using CTime2.Views.Settings;
using CTime2.Views.Statistics;
using CTime2.Views.Statistics.Details;
using CTime2.Views.Statistics.Details.BreakTime;
using CTime2.Views.Statistics.Details.EnterAndLeaveTime;
using CTime2.Views.Statistics.Details.OverTime;
using CTime2.Views.Statistics.Details.WorkTime;
using CTime2.Views.Terminal;
using CTime2.Views.UpdateNotes;
using CTime2.Views.YourTimes;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using UwCore.Application;
using UwCore.Extensions;
using UwCore.Hamburger;
using UwCore.Logging;
using UwCore.Services.Analytics;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using CTime2.Views.AddVacation;

namespace CTime2
{
    sealed partial class App
    {
        #region Logging
        private static readonly ILog Logger = LogManager.GetLog(typeof(App));
        #endregion

        #region Fields
        private Timer _appTimer;
        #endregion

        #region Constructors
        public App()
        {
            this.InitializeComponent();

            this.Suspending += this.App_OnSuspending;
            this.Resuming += this.App_OnResuming;
        }
        #endregion

        #region Configuration
        public override void Configure()
        {
            base.Configure();

            this.ConfigureVoiceCommands();
            this.ConfigureWindowMinSize();
            this.ConfigureJumpList();
        }

        private async void ConfigureVoiceCommands()
        {
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///CTime2.VoiceCommandService/CTime2VoiceCommands.xml"));
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
            }
            catch (Exception exception)
            {
                Logger.Warn($"For some reason the voice-command registration failed. {exception.GetFullMessage()}");
                Logger.Error(exception);
            }
        }

        private void ConfigureWindowMinSize()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(380, 500));
        }

        private async void ConfigureJumpList()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 2, 0) && JumpList.IsSupported())
            {
                JumpList jumpList = await JumpList.LoadCurrentAsync();
                jumpList.Items.Clear();

                var checkInArguments = new CTimeStartupArguments { Action = CTimeStartupAction.Checkin };
                var checkInItem = JumpListItem.CreateWithArguments(StartupArguments.AsString(checkInArguments), CTime2Resources.Get("StampHelper.CheckIn"));
                jumpList.Items.Add(checkInItem);

                var checkOutArguments = new CTimeStartupArguments { Action = CTimeStartupAction.Checkout };
                var checkOutItem = JumpListItem.CreateWithArguments(StartupArguments.AsString(checkOutArguments), CTime2Resources.Get("StampHelper.CheckOut"));
                jumpList.Items.Add(checkOutItem);

                await jumpList.SaveAsync();
            }
        }

        public override void CustomizeShell(IShell shell)
        {
            base.CustomizeShell(shell);

            shell.Theme = IoC.Get<IApplicationStateService>().GetApplicationTheme();

            shell.HeaderDetailsViewModel = IoC.Get<HeaderDetailsViewModel>();

            shell.SecondaryActions.Add(new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.About"), Symbol.ContactInfo, typeof(AboutViewModel)));
            shell.SecondaryActions.Add(new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Settings"), Symbol.Setting, typeof(SettingsViewModel)));
        }

        public override bool UseNewShellIfPossible()
        {
            return true;
        }
        #endregion

        #region Lifecycle
        public override ShellMode GetCurrentMode()
        {
            return IoC.Get<IApplicationStateService>().GetCurrentUser() != null
                ? (ShellMode)IoC.Get<LoggedInApplicationMode>()
                : IoC.Get<LoggedOutApplicationMode>();
        }

        public override void AppStartupFinished()
        {
            async void Tick()
            {
                Logger.Info("AppTimer Tick Begin!");

                try
                {
                    await IoC.Get<ITileService>().UpdateLiveTileAsync();
                    await IoC.Get<IEmployeeNotificationService>().SendNotificationsAsync();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }

                Logger.Info("AppTimer Tick Finish!");
            }

            this._appTimer = new Timer(_ => Tick(), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

            this.UnRegisterBackgroundTasks();
        }

        private async void App_OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await this.RegisterBackgroungTasks();

            deferral.Complete();
        }

        private void App_OnResuming(object sender, object e)
        {
            this.UnRegisterBackgroundTasks();
        }
        #endregion

        #region Errors
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
        #endregion

        #region Dependency Injection
        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(LoginViewModel);
            yield return typeof(OverviewViewModel);
            yield return typeof(YourTimesViewModel);
            yield return typeof(AboutViewModel);
            yield return typeof(AttendanceListViewModel);
            yield return typeof(StatisticsViewModel);
            yield return typeof(CheckedInViewModel);
            yield return typeof(CheckedOutViewModel);
            yield return typeof(SettingsViewModel);
            yield return typeof(HeaderDetailsViewModel);
            yield return typeof(DetailedStatisticViewModel);
            yield return typeof(UpdateNotesViewModel);
            yield return typeof(GeoLocationInfoViewModel);
            yield return typeof(MyTimeViewModel);
            yield return typeof(AttendingUserDetailsViewModel);
            yield return typeof(TerminalViewModel);
            yield return typeof(BreakTimeViewModel);
            yield return typeof(EnterAndLeaveTimeViewModel);
            yield return typeof(OverTimeViewModel);
            yield return typeof(WorkTimeViewModel);
            yield return typeof(AddVacationViewModel);
        }

        public override IEnumerable<Type> GetShellModeTypes()
        {
            yield return typeof(LoggedOutApplicationMode);
            yield return typeof(LoggedInApplicationMode);
            yield return typeof(TerminalApplicationMode);
        }

        public override IEnumerable<Type> GetServiceTypes()
        {
            yield return typeof(ICTimeService);
            yield return typeof(CTimeService);

            yield return typeof(ILicensesService);
            yield return typeof(LicensesService);

            yield return typeof(IBiometricsService);
            yield return typeof(BiometricsService);

            yield return typeof(IGeoLocationService);
            yield return typeof(GeoLocationService);

            yield return typeof(IContactsService);
            yield return typeof(ContactsService);

            yield return typeof(IEmployeeGroupService);
            yield return typeof(EmployeeGroupService);

            yield return typeof(IPhoneService);
            yield return typeof(PhoneService);

            yield return typeof(IEmailService);
            yield return typeof(EmailService);

            yield return typeof(ISharingService);
            yield return typeof(SharingService);

            yield return typeof(IStatisticsService);
            yield return typeof(StatisticsService);

            yield return typeof(ITileService);
            yield return typeof(TileService);

            yield return typeof(ICTimeRequestCache);
            yield return typeof(CTimeRequestCache);

            yield return typeof(IEmployeeNotificationService);
            yield return typeof(Core.Services.EmployeeNotification.EmployeeNotificationService);
        }
        #endregion

        #region Analytics
        public override IAnalyticsService GetAnalyticsService()
        {
#if STORE
            string appCenterSecret = "26792bb8-2428-4009-9ea2-573c46d15a77";
#else
            string appCenterSecret = "47d68127-8259-42ba-9449-e971c0bbabb3";
#endif

            return new AppCenterAnalyticsService(appCenterSecret, typeof(Analytics), typeof(Crashes));
        }

        public override bool IsAnalyticsServiceEnabled()
        {
            return base.IsAnalyticsServiceEnabled() &&
                   Package.Current.Id.Version.ToVersion().Revision == 0;
        }
        #endregion

        #region Update Notes
        public override Type GetUpdateNotesViewModelType()
        {
            return typeof(UpdateNotesViewModel);
        }
        #endregion

        #region Background Tasks
        private async Task RegisterBackgroungTasks()
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();

            var validStatus = new[]
            {
                BackgroundAccessStatus.AlwaysAllowed,
                BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity,
                BackgroundAccessStatus.AllowedSubjectToSystemPolicy,
                BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity,
            };

            if (validStatus.Contains(access))
            {
                BackgroundTaskHelper.Register(
                    typeof(CTime2LiveTileService),
                    new TimeTrigger(15, false),
                    forceRegister: true,
                    enforceConditions: true,
                    conditions: new SystemCondition(SystemConditionType.InternetAvailable));

                BackgroundTaskHelper.Register(
                    typeof(CTime2EmployeeNotificationService),
                    new TimeTrigger(15, false),
                    forceRegister: true,
                    enforceConditions: true,
                    conditions: new SystemCondition(SystemConditionType.InternetAvailable));
            }
        }

        private void UnRegisterBackgroundTasks()
        {
            BackgroundTaskHelper.Unregister(typeof(CTime2LiveTileService));
            BackgroundTaskHelper.Unregister(typeof(CTime2EmployeeNotificationService));
        }
        #endregion
    }
}

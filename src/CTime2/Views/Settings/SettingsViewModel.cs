using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings
{
    public class SettingsViewModel : UwCoreScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IShell _shell;

        private ReactiveList<TimeSpan> _workTimes;
        private TimeSpan _selectedWorkTime;
        private ReactiveList<TimeSpan> _breakTimes;
        private TimeSpan _selectedBreakTime;
        private ElementTheme _theme;
        private string _companyId;

        public ReactiveList<TimeSpan> WorkTimes
        {
            get { return this._workTimes; }
            set { this.RaiseAndSetIfChanged(ref this._workTimes, value); }
        }

        public TimeSpan SelectedWorkTime
        {
            get { return this._selectedWorkTime; }
            set { this.RaiseAndSetIfChanged(ref this._selectedWorkTime, value); }
        }

        public ReactiveList<TimeSpan> BreakTimes
        {
            get { return this._breakTimes; }
            set { this.RaiseAndSetIfChanged(ref this._breakTimes, value); }
        }

        public TimeSpan SelectedBreakTime
        {
            get { return this._selectedBreakTime; }
            set { this.RaiseAndSetIfChanged(ref this._selectedBreakTime, value); }
        }

        public ElementTheme Theme
        {
            get { return this._theme; }
            set { this.RaiseAndSetIfChanged(ref this._theme, value); }
        }

        public string CompanyId
        {
            get { return this._companyId; }
            set { this.RaiseAndSetIfChanged(ref this._companyId, value); }
        }
        
        public UwCoreCommand<Unit> RememberLogin { get; }

        public SettingsViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService, IShell shell)
        {
            Guard.NotNull(biometricsService, nameof(biometricsService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(shell, nameof(shell));

            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;
            this._shell = shell;

            var hasUser = new ReplaySubject<bool>(1);
            hasUser.OnNext(this._applicationStateService.GetCurrentUser() != null);
            var deviceAvailable = this._biometricsService.BiometricAuthDeviceIsAvailableAsync().ToObservable();
            var canRememberLogin = hasUser.CombineLatest(deviceAvailable, (hasUsr, deviceAvail) => hasUsr && deviceAvail);
            this.RememberLogin = UwCoreCommand.Create(canRememberLogin, this.RememberLoginImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.RememberedLogin"))
                .HandleExceptions()
                .TrackEvent("SetupRememberLogin");
            
            this.Theme = this._applicationStateService.GetApplicationTheme();
            this.SelectedWorkTime = this._applicationStateService.GetWorkDayHours();
            this.SelectedBreakTime = this._applicationStateService.GetWorkDayBreak();
            this.CompanyId = this._applicationStateService.GetCompanyId();

            this.WhenAnyValue(f => f.Theme)
                .Subscribe(theme =>
                {
                    this._shell.Theme = theme;
                    this._applicationStateService.SetApplicationTheme(theme);
                });

            this.WhenAnyValue(f => f.SelectedWorkTime)
                .Subscribe(workTime =>
                {
                    this._applicationStateService.SetWorkDayHours(workTime);
                });

            this.WhenAnyValue(f => f.SelectedBreakTime)
                .Subscribe(breakTime =>
                {
                    this._applicationStateService.SetWorkDayBreak(breakTime);
                });

            this.WhenAnyValue(f => f.CompanyId)
                .Subscribe(companyId =>
                {
                    this._applicationStateService.SetCompanyId(companyId);
                });

            this.DisplayName = CTime2Resources.Get("Navigation.Settings");

            this.WorkTimes = new ReactiveList<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));

            this.BreakTimes = new ReactiveList<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));
        }
        
        private async Task RememberLoginImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();
            await this._biometricsService.RememberUserForBiometricAuthAsync(user);
        }
    }
}
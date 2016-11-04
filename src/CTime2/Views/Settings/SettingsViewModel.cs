using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.Biometrics;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings
{
    public class SettingsViewModel : ReactiveScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IBandService _bandService;
        private readonly IApplication _application;

        private ReactiveObservableCollection<TimeSpan> _workTimes;
        private TimeSpan _selectedWorkTime;
        private ReactiveObservableCollection<TimeSpan> _breakTimes;
        private TimeSpan _selectedBreakTime;
        private BandState _state;
        private ElementTheme _theme;
        private string _companyId;

        public ReactiveObservableCollection<TimeSpan> WorkTimes
        {
            get { return this._workTimes; }
            set { this.RaiseAndSetIfChanged(ref this._workTimes, value); }
        }

        public TimeSpan SelectedWorkTime
        {
            get { return this._selectedWorkTime; }
            set { this.RaiseAndSetIfChanged(ref this._selectedWorkTime, value); }
        }

        public ReactiveObservableCollection<TimeSpan> BreakTimes
        {
            get { return this._breakTimes; }
            set { this.RaiseAndSetIfChanged(ref this._breakTimes, value); }
        }

        public TimeSpan SelectedBreakTime
        {
            get { return this._selectedBreakTime; }
            set { this.RaiseAndSetIfChanged(ref this._selectedBreakTime, value); }
        }

        public BandState State
        {
            get { return this._state; }
            set { this.RaiseAndSetIfChanged(ref this._state, value); }
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

        public UwCoreCommand<Unit> Reload { get; }
        public UwCoreCommand<Unit> RememberLogin { get; }
        public UwCoreCommand<Unit> ToggleBandTile { get; }

        public SettingsViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService, IBandService bandService, IApplication application)
        {
            Guard.NotNull(biometricsService, nameof(biometricsService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(bandService, nameof(bandService));

            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;
            this._bandService = bandService;
            this._application = application;

            var hasUser = new ReplaySubject<bool>(1);
            hasUser.OnNext(this._applicationStateService.GetCurrentUser() != null);
            var deviceAvailable = this._biometricsService.BiometricAuthDeviceIsAvailableAsync().ToObservable();
            var canRememberLogin = hasUser.CombineLatest(deviceAvailable, (hasUsr, deviceAvail) => hasUsr && deviceAvail);
            this.RememberLogin = UwCoreCommand.Create(canRememberLogin, this.RememberLoginImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.RememberedLogin"))
                .HandleExceptions()
                .TrackEvent("SetupRememberLogin");

            var canToggleTile = this.WhenAnyValue(f => f.State, state => state != BandState.NotConnected);
            this.ToggleBandTile = UwCoreCommand.Create(canToggleTile, this.ToggleTileImpl)
                .ShowLoadingOverlay(() => this.State == BandState.Installed
                    ? CTime2Resources.Get("Loading.RemoveTileFromBand")
                    : CTime2Resources.Get("Loading.AddTileToBand"))
                .HandleExceptions()
                .TrackEvent("ToggleBandTile");

            this.Reload = UwCoreCommand.Create(this.ReloadImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Settings"))
                .HandleExceptions()
                .TrackEvent("ReloadSettings");

            this.Theme = this._applicationStateService.GetApplicationTheme();
            this.SelectedWorkTime = this._applicationStateService.GetWorkDayHours();
            this.SelectedBreakTime = this._applicationStateService.GetWorkDayBreak();
            this.CompanyId = this._applicationStateService.GetCompanyId();

            this.WhenAnyValue(f => f.Theme)
                .Subscribe(theme =>
                {
                    this._application.Theme = theme;
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

            this.WorkTimes = new ReactiveObservableCollection<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));

            this.BreakTimes = new ReactiveObservableCollection<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.Reload.ExecuteAsync();
        }

        private async Task RememberLoginImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();
            await this._biometricsService.RememberUserForBiometricAuthAsync(user);
        }

        private async Task ToggleTileImpl()
        {
            if (this.State == BandState.NotConnected)
                return;

            if (this.State == BandState.Installed)
            {
                await this._bandService.UnRegisterBandTileAsync();
            }
            else
            {
                await this._bandService.RegisterBandTileAsync();
            }

            await this.ReloadImpl();
        }

        private async Task ReloadImpl()
        {
            var bandInfo = await this._bandService.GetBand();

            if (bandInfo == null)
            {
                this.State = BandState.NotConnected;
                return;
            }

            if (await this._bandService.IsBandTileRegisteredAsync())
            {
                this.State = BandState.Installed;
            }
            else
            {
                this.State = BandState.NotInstalled;
            }
        }
    }
}
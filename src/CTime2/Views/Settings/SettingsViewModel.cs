using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.Biometrics;
using CTime2.Strings;
using CTime2.Views.About;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings
{
    public class SettingsViewModel : ReactiveScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IBandService _bandService;

        private ReactiveObservableCollection<TimeSpan> _workTimes;
        private TimeSpan _selectedWorkTime;
        private ReactiveObservableCollection<TimeSpan> _breakTimes;
        private TimeSpan _selectedBreakTime;
        private BandState _state;

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

        public ReactiveCommand<Unit> Reload { get; }
        public ReactiveCommand<Unit> RememberLogin { get; }
        public ReactiveCommand<Unit> ToggleBandTile { get; }

        public SettingsViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService, IBandService bandService)
        {
            Guard.NotNull(biometricsService, nameof(biometricsService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(bandService, nameof(bandService));

            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;
            this._bandService = bandService;

            var canRememberLogin = new ReplaySubject<bool>(1);
            canRememberLogin.OnNext(this._applicationStateService.GetCurrentUser() != null);
            this.RememberLogin = ReactiveCommand.CreateAsyncTask(canRememberLogin, _ => this.RememberLoginImpl());
            this.RememberLogin.AttachExceptionHandler();
            this.RememberLogin.AttachLoadingService(CTime2Resources.Get("Loading.RememberedLogin"));
            this.RememberLogin.TrackEvent("SetupRememberLogin");

            var canToggleTile = this.WhenAnyValue(f => f.State, state => state != BandState.NotConnected);
            this.ToggleBandTile = ReactiveCommand.CreateAsyncTask(canToggleTile, _ => this.ToggleTileImpl());
            this.ToggleBandTile.AttachLoadingService(() => this.State == BandState.Installed
                    ? CTime2Resources.Get("Loading.RemoveTileFromBand")
                    : CTime2Resources.Get("Loading.AddTileToBand"));
            this.ToggleBandTile.AttachExceptionHandler();
            this.ToggleBandTile.TrackEvent("ToggleBandTile");

            this.Reload = ReactiveCommand.CreateAsyncTask(_ => this.ReloadImpl());
            this.Reload.AttachLoadingService(CTime2Resources.Get("Loading.Settings"));
            this.Reload.AttachExceptionHandler();
            this.Reload.TrackEvent("ReloadBandInfo");

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
            this.SelectedWorkTime = this._applicationStateService.GetWorkDayHours();
            this.SelectedBreakTime = this._applicationStateService.GetWorkDayBreak();

            await this.Reload.ExecuteAsyncTask();
        }

        protected override void OnDeactivate(bool close)
        {
            this._applicationStateService.SetWorkDayHours(this.SelectedWorkTime);
            this._applicationStateService.SetWorkDayBreak(this.SelectedBreakTime);
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
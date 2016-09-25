using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings.Others
{
    public class OthersViewModel : ReactiveScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;

        private ReactiveObservableCollection<TimeSpan> _workTimes;
        private TimeSpan _selectedWorkTime;
        private ReactiveObservableCollection<TimeSpan> _breakTimes;
        private TimeSpan _selectedBreakTime;
        
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

        public ReactiveCommand<Unit> RememberLogin { get; }

        public OthersViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(biometricsService, nameof(biometricsService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;
            
            this.WorkTimes = new ReactiveObservableCollection<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));
            this.BreakTimes = new ReactiveObservableCollection<TimeSpan>(Enumerable
                .Repeat((object)null, 4 * 24)
                .Select((_, i) => TimeSpan.FromHours(0.25 * i)));

            var canRememberLogin = new ReplaySubject<bool>(1);
            canRememberLogin.OnNext(this._applicationStateService.GetCurrentUser() != null);

            this.RememberLogin = ReactiveCommand.CreateAsyncTask(canRememberLogin, _ => this.RememberLoginImpl());
            this.RememberLogin.AttachExceptionHandler();
            this.RememberLogin.AttachLoadingService(CTime2Resources.Get("Loading.RememberedLogin"));
            this.RememberLogin.TrackEvent("SetupRememberLogin");

            this.DisplayName = CTime2Resources.Get("Navigation.Others");
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

        protected override void OnActivate()
        {
            this.SelectedWorkTime = this._applicationStateService.GetWorkDayHours();
            this.SelectedBreakTime = this._applicationStateService.GetWorkDayBreak();
        }
    }
}
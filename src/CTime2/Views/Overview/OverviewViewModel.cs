using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.GeoLocation;
using CTime2.Strings;
using CTime2.Views.GeoLocationInfo;
using CTime2.Views.Overview.CheckedIn;
using CTime2.Views.Overview.CheckedOut;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Navigation;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : UwCoreConductor<StampTimeStateViewModelBase>, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly INavigationService _navigationService;
        
        private string _welcomeMessage;
        private byte[] _myImage;
        private GeoLocationState _geoLocationState;
        private MyTimeViewModel _myTimeViewModel;
        private bool _warnForMissingDays;

        public string WelcomeMessage
        {
            get { return this._welcomeMessage; }
            set { this.RaiseAndSetIfChanged(ref this._welcomeMessage, value); }
        }

        public byte[] MyImage
        {
            get { return this._myImage; }
            set { this.RaiseAndSetIfChanged(ref this._myImage, value); }
        }

        public GeoLocationState GeoLocationState
        {
            get { return this._geoLocationState; }
            set { this.RaiseAndSetIfChanged(ref this._geoLocationState, value); }
        }

        public MyTimeViewModel MyTimeViewModel
        {
            get { return this._myTimeViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._myTimeViewModel, value); }
        }

        public bool WarnForMissingDays
        {
            get { return this._warnForMissingDays; }
            set { this.RaiseAndSetIfChanged(ref this._warnForMissingDays, value); }
        }

        public UwCoreCommand<Unit> RefreshCurrentState { get; }
        public UwCoreCommand<Unit> ShowGeoLocationInfo { get; }
        public UwCoreCommand<Unit> GoToMyTimesWithMissingDays { get; }

        public OverviewViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator, IGeoLocationService geoLocationService, INavigationService navigationService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(geoLocationService, nameof(geoLocationService));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._geoLocationService = geoLocationService;
            this._navigationService = navigationService;

            this.DisplayName = CTime2Resources.Get("Navigation.Overview");

            this.RefreshCurrentState = UwCoreCommand.Create(this.RefreshCurrentStateImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CurrentState"))
                .HandleExceptions()
                .TrackEvent("RefreshCurrentState");

            this.ShowGeoLocationInfo = UwCoreCommand.Create(this.ShowGeoLocationInfoImpl)
                .HandleExceptions()
                .TrackEvent("GeoLocationInfo");

            this.GoToMyTimesWithMissingDays = UwCoreCommand.Create(this.WhenAnyValue(f => f.WarnForMissingDays), this.GoToMyTimesWithMissingDaysImpl)
                .HandleExceptions()
                .TrackEvent("GoToMyTimesWithMissingDays");

            this.MyTimeViewModel = IoC.Get<MyTimeViewModel>();
            this.MyTimeViewModel.ConductWith(this);

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            var currentUser = this._applicationStateService.GetCurrentUser();

            this.WelcomeMessage = CTime2Resources.GetFormatted("Overview.WelcomeMessageFormat", currentUser.FirstName);
            this.MyImage = currentUser.ImageAsPng;

            this.GeoLocationState = await this._geoLocationService.GetGeoLocationStateAsync(currentUser);
            
            await this.RefreshCurrentState.ExecuteAsync();
        }
        
        private async Task RefreshCurrentStateImpl()
        {
            await Task.WhenAll(
                this.RefreshCurrentlyCheckedIn(), 
                this.RefreshWarningForMissingDays());
        }

        private async Task RefreshWarningForMissingDays()
        {
            var employeeGuid = this._applicationStateService.GetCurrentUser().Id;
            var workDays = this._applicationStateService.GetWorkDays();

            var times = await this._cTimeService.GetTimes(employeeGuid, this.GetStartOfTwoWeeksAgo(), DateTime.Today);

            this.WarnForMissingDays = TimesByDay.Create(times, workDays).Any(f => f.IsMissing);
        }

        private async Task RefreshCurrentlyCheckedIn()
        {
            var employeeGuid = this._applicationStateService.GetCurrentUser().Id;

            var currentState = await this._cTimeService.IsCurrentlyCheckedIn(employeeGuid)
                ? (StampTimeStateViewModelBase)IoC.Get<CheckedInViewModel>()
                : IoC.Get<CheckedOutViewModel>();

            this.ActivateItem(currentState);
        }

        private DateTime GetStartOfTwoWeeksAgo()
        {
            // Looks ugly, is ugly, BUT IT WORKS!

            var current = DateTime.Today;

            // One week ago
            while (current.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
                current = current.AddDays(-1);

            current = current.AddDays(-1);

            // Two weeks ago
            while (current.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
                current = current.AddDays(-1);

            return current;
        }

        private Task ShowGeoLocationInfoImpl()
        {
            this._navigationService.Popup.For<GeoLocationInfoViewModel>().Navigate();

            return Task.CompletedTask;
        }

        private Task GoToMyTimesWithMissingDaysImpl()
        {
            this._navigationService.For<YourTimesViewModel>()
                .WithParam(f => f.Parameter.StartDate, this.GetStartOfTwoWeeksAgo())
                .WithParam(f => f.Parameter.EndDate, DateTime.Today)
                .Navigate();

            return Task.CompletedTask;
        }

        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }

        async Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }
    }
}
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.GeoLocation;
using CTime2.Extensions;
using CTime2.Strings;
using CTime2.Views.GeoLocationInfo;
using CTime2.Views.Overview.CheckedIn;
using CTime2.Views.Overview.CheckedOut;
using CTime2.Views.Overview.HomeOfficeCheckedIn;
using CTime2.Views.Overview.TripCheckedIn;
using ReactiveUI;
using UwCore;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : ReactiveConductor<StampTimeStateViewModelBase>, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly INavigationService _navigationService;

        private readonly Timer _timer;

        private DateTime _timerStartNow;
        private TimeSpan _timerStartTimeForDay;

        private string _welcomeMessage;
        private TimeSpan _currentTime;
        private byte[] _myImage;
        private GeoLocationState _geoLocationState;

        public string WelcomeMessage
        {
            get { return this._welcomeMessage; }
            set { this.RaiseAndSetIfChanged(ref this._welcomeMessage, value); }
        }

        public TimeSpan CurrentTime
        {
            get { return this._currentTime; }
            set { this.RaiseAndSetIfChanged(ref this._currentTime, value); }
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

        public UwCoreCommand<Unit> RefreshCurrentState { get; }
        public UwCoreCommand<Unit> ShowGeoLocationInfo { get; }

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

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            this.RefreshCurrentState = UwCoreCommand.Create(this.RefreshCurrentStateImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CurrentState"))
                .HandleExceptions()
                .TrackEvent("RefreshCurrentState");

            this.ShowGeoLocationInfo = UwCoreCommand.Create(this.ShowGeoLocationInfoImpl)
                .HandleExceptions()
                .TrackEvent("GeoLocationInfo");

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();

            this.WelcomeMessage = CTime2Resources.GetFormatted("Overview.WelcomeMessageFormat", currentUser.FirstName);
            this.MyImage = currentUser.ImageAsPng;

            this.GeoLocationState = await this._geoLocationService.GetGeoLocationStateAsync(currentUser);
            
            await this.RefreshCurrentState.ExecuteAsync();
        }
        
        private async Task RefreshCurrentStateImpl()
        {
            var currentTime = await this._cTimeService.GetCurrentTime(this._applicationStateService.GetCurrentUser().Id);

            #region Update State
            StampTimeStateViewModelBase currentState;

            if (currentTime == null || currentTime.State.IsLeft())
            {
                currentState = IoC.Get<CheckedOutViewModel>();
            }
            else if (currentTime.State.IsEntered() && currentTime.State.IsTrip())
            {
                currentState = IoC.Get<TripCheckedInViewModel>();
            }
            else if (currentTime.State.IsEntered() && currentTime.State.IsHomeOffice())
            {
                currentState = IoC.Get<HomeOfficeCheckedInViewModel>();
            }
            else if (currentTime.State.IsEntered())
            {
                currentState = IoC.Get<CheckedInViewModel>();
            }
            else
            {
                throw new CTimeException("Could not determine the current state.");
            }

            this.ActivateItem(currentState);
            #endregion

            #region Update Timer
            this._timerStartNow = DateTime.Now;

            var timeToAdd = currentTime != null && currentTime.State.IsEntered()
                ? this._timerStartNow - (currentTime.ClockInTime ?? this._timerStartNow)
                : TimeSpan.Zero;

            var timeToday = currentTime?.Hours ?? TimeSpan.Zero;

            this.CurrentTime = this._timerStartTimeForDay = timeToday + timeToAdd;

            if (currentTime != null && currentTime.State.IsEntered())
            {
                this._timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            }
            else
            {
                this._timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }
            #endregion
        }

        private Task ShowGeoLocationInfoImpl()
        {
            this._navigationService.Popup.For<GeoLocationInfoViewModel>().Navigate();

            return Task.CompletedTask;
        }

        private void Tick(object state)
        {
            Execute.OnUIThread(() =>
            {
                this.CurrentTime = this._timerStartTimeForDay + (DateTime.Now - this._timerStartNow);
            });
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
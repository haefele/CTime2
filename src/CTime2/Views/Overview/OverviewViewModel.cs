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
using CTime2.Extensions;
using CTime2.Strings;
using CTime2.Views.Overview.CheckedIn;
using CTime2.Views.Overview.CheckedOut;
using CTime2.Views.Overview.HomeOfficeCheckedIn;
using CTime2.Views.Overview.TripCheckedIn;
using ReactiveUI;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : ReactiveConductor<StampTimeStateViewModelBase>, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;

        private readonly Timer _timer;

        private DateTime _timerStartNow;
        private TimeSpan _timerStartTimeForDay;

        private string _welcomeMessage;
        private TimeSpan _currentTime;
        private byte[] _myImage;

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
        
        public ReactiveCommand<Unit> RefreshCurrentState { get; }

        public OverviewViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;

            this.DisplayName = CTime2Resources.Get("Navigation.Overview");

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            
            this.RefreshCurrentState = ReactiveCommand.CreateAsyncTask(_ => this.RefreshCurrentStateImpl());
            this.RefreshCurrentState.AttachExceptionHandler();
            this.RefreshCurrentState.AttachLoadingService(CTime2Resources.Get("Loading.CurrentState"));

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            this.WelcomeMessage = CTime2Resources.GetFormatted("Overview.WelcomeMessageFormat", this._applicationStateService.GetCurrentUser().FirstName);
            this.MyImage = this._applicationStateService.GetCurrentUser().ImageAsPng;
            
            await this.RefreshCurrentState.ExecuteAsyncTask();
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

        private void Tick(object state)
        {
            Execute.OnUIThread(() =>
            {
                this.CurrentTime = this._timerStartTimeForDay + (DateTime.Now - this._timerStartNow);
            });
        }

        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.RefreshCurrentState.ExecuteAsyncTask();
        }

        async Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            await this.RefreshCurrentState.ExecuteAsyncTask();
        }
    }
}
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
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
    public class MyTimeViewModel : UwCoreScreen, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly INavigationService _navigationService;

        private readonly Timer _timer;

        private DateTime _timerStartNow;
        private TimeSpan _timerTimeForDay;

        private TimeSpan _currentTime;
        private TimeSpan? _overTime;

        public TimeSpan CurrentTime
        {
            get { return this._currentTime; }
            set { this.RaiseAndSetIfChanged(ref this._currentTime, value); }
        }

        public TimeSpan? OverTime
        {
            get { return this._overTime; }
            set { this.RaiseAndSetIfChanged(ref this._overTime, value); }
        }

        public UwCoreCommand<Unit> RefreshTimer { get; }

        public UwCoreCommand<Unit> GoToMyTimes { get; }

        public MyTimeViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator, INavigationService navigationService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._navigationService = navigationService;

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            this.RefreshTimer = UwCoreCommand.Create(this.RefreshTimerImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CurrentTime"))
                .HandleExceptions()
                .TrackEvent("RefreshTimer");

            this.GoToMyTimes = UwCoreCommand.Create(this.GoToMyTimesImpl)
                .HandleExceptions()
                .TrackEvent("TimerGoToMyTimes");

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.RefreshTimer.ExecuteAsync();
        }

        private async Task RefreshTimerImpl()
        {
            var currentTime = await this._cTimeService.GetCurrentTime(this._applicationStateService.GetCurrentUser().Id);
            
            this._timerStartNow = DateTime.Now;

            var timeToAdd = currentTime != null && currentTime.State.IsEntered()
                ? this._timerStartNow - (currentTime.ClockInTime ?? this._timerStartNow)
                : TimeSpan.Zero;

            //Make sure we never count down, always up
            if (timeToAdd < TimeSpan.Zero)
                timeToAdd = TimeSpan.Zero;

            var timeToday = currentTime?.Hours ?? TimeSpan.Zero;

            this.SetTime(this._timerTimeForDay = timeToday + timeToAdd);

            if (currentTime != null && currentTime.State.IsEntered())
            {
                this._timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            }
            else
            {
                this._timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }
        }

        private Task GoToMyTimesImpl()
        {
            this._navigationService.For<YourTimesViewModel>()
                .WithParam(f => f.StartDate, DateTimeOffset.Now.WithoutTime())
                .WithParam(f => f.EndDate, DateTimeOffset.Now.WithoutTime())
                .Navigate();

            return Task.CompletedTask;
        }

        private void Tick(object state)
        {
            Execute.OnUIThread(() =>
            {
                this.SetTime(this._timerTimeForDay + (DateTime.Now - this._timerStartNow));
            });
        }

        private void SetTime(TimeSpan time)
        {
            var workTime = this._applicationStateService.GetWorkDayHours();

            if (time - workTime > TimeSpan.FromSeconds(1))
            {
                this.CurrentTime = workTime;
                this.OverTime = time - workTime;
            }
            else
            {
                this.CurrentTime = time;
                this.OverTime = null;
            }
        }

        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.RefreshTimer.ExecuteAsync();
        }

        async Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            await this.RefreshTimer.ExecuteAsync();
        }
    }
}
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
using CTime2.Views.About;
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

        private TimeSpan? _lunchBreakTime;
        private DateTime _lunchBreakStart;
        private DateTime _preferedLunchBreakEnd;

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

        public TimeSpan? LunchBreakTime
        {
            get { return this._lunchBreakTime; }
            set { this.RaiseAndSetIfChanged(ref this._lunchBreakTime, value); }
        }
        
        public DateTime PreferedLunchBreakEnd
        {
            get { return this._preferedLunchBreakEnd; }
            set { this.RaiseAndSetIfChanged(ref this._preferedLunchBreakEnd, value); }
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

            //Only take the timeToday if the time is either
            // - from today
            // - or from yesterday, but still checked-in
            var timeToday = currentTime?.Day == DateTime.Today || (currentTime?.State.IsEntered() ?? false)
                ? currentTime.Hours 
                : TimeSpan.Zero;

            this.SetTime(this._timerTimeForDay = timeToday + timeToAdd);

            if (this.IsLunchBreak(currentTime))
            {
                this.LunchBreakTime = TimeSpan.MinValue;

                // this will result in the timer starting between 00:00 and 00:59 due to server rounding down to the full minute

                this._lunchBreakStart = currentTime.ClockOutTime.Value;

                var preferedBreakLenght = this._applicationStateService.GetWorkDayBreak();

                this.PreferedLunchBreakEnd = this._lunchBreakStart + preferedBreakLenght;

                // making sure the timer is started

                this._timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            }
            else
            {
                if (this.LunchBreakTime != null)
                {
                    this.LunchBreakTime = null;
                    this._lunchBreakStart = DateTime.MinValue;
                }

                if (currentTime != null && currentTime.State.IsEntered())
                {
                    this._timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    this._timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        private bool IsLunchBreak(Time currentTime)
        {
            if (currentTime == null || currentTime.State != TimeState.Left || currentTime.ClockOutTime.HasValue == false)
                return false;

            if (currentTime.ClockOutTime.Value < DateTime.Parse("11:00") || currentTime.ClockOutTime.Value > DateTime.Parse("14:30"))
                return false;

            return true;
        }

        private Task GoToMyTimesImpl()
        {
            this._navigationService.For<YourTimesViewModel>()
                .WithParam(f => f.Parameter.StartDate, DateTimeOffset.Now.WithoutTime())
                .WithParam(f => f.Parameter.EndDate, DateTimeOffset.Now.WithoutTime())
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
            if (this.LunchBreakTime != null && DateTime.Now > DateTime.Parse("14:30"))
            {
                this.LunchBreakTime = null;
                this._lunchBreakStart = DateTime.MinValue;
            }

            if (this.LunchBreakTime != null)
            {
                this.LunchBreakTime = DateTime.Now - this._lunchBreakStart;
            }
            else
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
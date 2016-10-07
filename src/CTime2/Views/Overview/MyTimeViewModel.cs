using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview
{
    public class MyTimeViewModel : ReactiveScreen, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
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

        public MyTimeViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            this.RefreshTimer = UwCoreCommand.Create(this.RefreshTimerImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CurrentTime"))
                .HandleExceptions()
                .TrackEvent("RefreshTimer");

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
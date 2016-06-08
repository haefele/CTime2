using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Extensions;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : ReactiveScreen, IHandleWithTask<ApplicationResumed>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IEventAggregator _eventAggregator;

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

        public ReactiveCommand<Unit> LoadCurrentTime { get; }

        public OverviewViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._eventAggregator = eventAggregator;

            this.DisplayName = CTime2Resources.Get("Navigation.Overview");

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            this.LoadCurrentTime = ReactiveCommand.CreateAsyncTask(_ => this.LoadCurrentTimeImpl());
            this.LoadCurrentTime.AttachExceptionHandler();

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            this.WelcomeMessage = CTime2Resources.GetFormatted("Overview.WelcomeMessageFormat", this._applicationStateService.GetCurrentUser().FirstName);
            this.MyImage = this._applicationStateService.GetCurrentUser().ImageAsPng;
            
            await this.LoadCurrentTime.ExecuteAsyncTask();
        }
        
        private async Task LoadCurrentTimeImpl()
        {
            Time current = await this._cTimeService.GetCurrentTime(this._applicationStateService.GetCurrentUser().Id);

            this._timerStartNow = DateTime.Now;

            var timeToAdd = current != null && current.State.IsEntered()
                ? this._timerStartNow - (current.ClockInTime ?? this._timerStartNow)
                : TimeSpan.Zero;

            var timeToday = current?.Hours ?? TimeSpan.Zero;

            this.CurrentTime = this._timerStartTimeForDay = timeToday + timeToAdd;

            if (current != null && current.State.IsEntered())
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
                this.CurrentTime = this._timerStartTimeForDay + (DateTime.Now - this._timerStartNow);
            });
        }

        Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            return this.LoadCurrentTime.ExecuteAsyncTask();
        }
    }
}
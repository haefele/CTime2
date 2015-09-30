using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Events;
using CTime2.Extensions;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : Screen, IHandleWithTask<ApplicationResumedEvent>
    {
        private readonly ISessionStateService _sessionStateService;
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
            set { this.SetProperty(ref this._welcomeMessage, value); }
        }

        public TimeSpan CurrentTime
        {
            get { return this._currentTime; }
            set { this.SetProperty(ref this._currentTime, value); }
        }

        public byte[] MyImage
        {
            get { return this._myImage; }
            set { this.SetProperty(ref this._myImage, value); }
        }

        public OverviewViewModel(ISessionStateService sessionStateService, ICTimeService cTimeService, IEventAggregator eventAggregator)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
            this._eventAggregator = eventAggregator;

            this.DisplayName = "Übersicht";

            this._timer = new Timer(this.Tick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
        }

        protected override async void OnActivate()
        {
            this._eventAggregator.Subscribe(this);
            
            this.WelcomeMessage = $"Hallo {this._sessionStateService.CurrentUser.FirstName}!";
            this.MyImage = this._sessionStateService.CurrentUser.ImageAsPng;
            
            await this.LoadCurrentTime();
        }

        protected override void OnDeactivate(bool close)
        {
            this._eventAggregator.Unsubscribe(this);
        }

        private async Task LoadCurrentTime()
        {
            Time current = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);

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
            this.CurrentTime = this._timerStartTimeForDay + (DateTime.Now - this._timerStartNow);
        }

        Task IHandleWithTask<ApplicationResumedEvent>.Handle(ApplicationResumedEvent message)
        {
            return this.LoadCurrentTime();
        }
    }
}
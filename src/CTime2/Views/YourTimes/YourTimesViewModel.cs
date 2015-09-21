using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Services.CTime;
using CTime2.Services.Loading;
using CTime2.Services.SessionState;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : Screen
    {
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        private readonly ILoadingService _loadingService;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public BindableCollection<Time> Times { get; }

        public DateTimeOffset StartDate
        {
            get { return this._startDate; }
            set { this.SetProperty(ref this._startDate, value); }
        }

        public DateTimeOffset EndDate
        {
            get { return this._endDate; }
            set { this.SetProperty(ref this._endDate, value); }
        }

        public YourTimesViewModel(ISessionStateService sessionStateService, ICTimeService cTimeService, ILoadingService loadingService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
            this._loadingService = loadingService;

            this.Times = new BindableCollection<Time>();

            this.StartDate = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, DateTimeOffset.Now.Offset);
            this.EndDate = DateTimeOffset.Now;
        }

        protected override async void OnActivate()
        {
            await this.RefreshAsync();
        }
        
        public async Task RefreshAsync()
        {
            using (this._loadingService.Show("Lade Zeiten..."))
            {
                var times = await this._cTimeService.GetTimes(this._sessionStateService.CurrentUser.Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

                this.Times.Clear();
                this.Times.AddRange(times);
            }
        }
    }
}
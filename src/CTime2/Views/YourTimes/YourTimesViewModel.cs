using System;
using Caliburn.Micro;
using CTime2.Services.CTime;
using CTime2.Services.SessionState;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : Screen
    {
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;

        public BindableCollection<Time> Times { get; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public YourTimesViewModel(ISessionStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;

            this.Times = new BindableCollection<Time>();

            this.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            this.EndDate = DateTime.Today;
        }

        protected override async void OnActivate()
        {
            var times = await this._cTimeService.GetTimes(this._sessionStateService.CurrentUser.Id, this.StartDate, this.EndDate);

            this.Times.AddRange(times);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }
    }
}
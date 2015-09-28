using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Extensions;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Dialog;
using CTime2.Services.Loading;
using CTime2.Views.YourTimes;

namespace CTime2.Views.Statistics
{
    public class StatisticsViewModel : Screen
    {
        #region Fields

        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        private readonly ILoadingService _loadingService;
        private readonly IDialogService _dialogService;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        #endregion

        #region Properties

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

        public BindableCollection<StatisticItem> Statistics { get; }

        #endregion

        #region Ctor

        public StatisticsViewModel(ISessionStateService sessionStateService, ICTimeService cTimeService, ILoadingService loadingService, IDialogService dialogService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
            this._loadingService = loadingService;
            this._dialogService = dialogService;

            this.DisplayName = "Statistiken";

            this.Statistics = new BindableCollection<StatisticItem>();

            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }

        #endregion

        #region Methods

        protected override async void OnActivate()
        {
            await this.RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            using (this._loadingService.Show("Lade Statistiken..."))
            {
                var times = await this._cTimeService.GetTimes(this._sessionStateService.CurrentUser.Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

                if (times.Count == 0)
                {
                    await this._dialogService.ShowAsync("In der gewählten Zeitspanne sind keine Zeiten verfügbar");
                    return;
                }
                
                var timesByDay = TimesByDay.Create(times)
                    .Where(this.IsTimeByDayForStatistic)
                    .ToList();

                var totalWorkTime = TimeSpan.FromMinutes(timesByDay.Where(f => f.Hours != TimeSpan.Zero).Sum(f => f.Hours.TotalMinutes));

                var totalWorkDays = timesByDay.Count(f => f.Hours != TimeSpan.Zero);

                var averageWorkTime = TimeSpan.FromMinutes(totalWorkTime.TotalMinutes / totalWorkDays);

                var averageEnterTime = TimeSpan.FromMinutes(
                    timesByDay.Where(f => f.DayStartTime != null).Sum(f => f.DayStartTime.Value.TotalMinutes) /
                    timesByDay.Count(f => f.DayStartTime != null));

                var averageLeaveTime = TimeSpan.FromMinutes(
                    timesByDay.Where(f => f.DayEndTime != null).Sum(f => f.DayEndTime.Value.TotalMinutes) /
                    timesByDay.Count(f => f.DayEndTime != null));

                var averageBreakTime = averageLeaveTime - averageEnterTime - averageWorkTime;

                this.Statistics.Clear();

                this.Statistics.Add(new StatisticItem("ø Arbeitszeit", averageWorkTime.ToString(@"hh\:mm\:ss")));
                this.Statistics.Add(new StatisticItem("ø Pausenlänge", averageBreakTime.ToString(@"hh\:mm\:ss")));
                this.Statistics.Add(new StatisticItem("ø Arbeitsbeginn", averageEnterTime.ToString(@"hh\:mm\:ss")));
                this.Statistics.Add(new StatisticItem("ø Arbeitsende", averageLeaveTime.ToString(@"hh\:mm\:ss")));
                this.Statistics.Add(new StatisticItem("Arbeitstage", totalWorkDays.ToString()));
                this.Statistics.Add(new StatisticItem("Gesamt Arbeitszeit", totalWorkTime.ToString(@"d\ \T\a\g\e\ hh\:mm\:ss")));
            }
        }
        private bool IsTimeByDayForStatistic(TimesByDay timesByDay)
        {
            if (timesByDay.Day != DateTime.Today)
                return true;

            if (timesByDay.Times.Count >= 2)
                return true;

            return false;
        }
        #endregion
    }
}
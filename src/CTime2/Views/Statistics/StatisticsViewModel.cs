using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.Statistics
{
    public class StatisticsViewModel : ReactiveScreen
    {
        #region Fields
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IDialogService _dialogService;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<StatisticItem>> _statisticsHelper;
        #endregion

        #region Properties
        public DateTimeOffset StartDate
        {
            get { return this._startDate; }
            set { this.RaiseAndSetIfChanged(ref this._startDate, value); }
        }
        public DateTimeOffset EndDate
        {
            get { return this._endDate; }
            set { this.RaiseAndSetIfChanged(ref this._endDate, value); }
        }

        public ReactiveObservableCollection<StatisticItem> Statistics => this._statisticsHelper.Value;
        #endregion

        #region Commands
        public ReactiveCommand<ReactiveObservableCollection<StatisticItem>> LoadStatistics { get; }
        #endregion

        #region Constructors
        public StatisticsViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IDialogService dialogService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(dialogService, nameof(dialogService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._dialogService = dialogService;

            this.DisplayName = CTime2Resources.Get("Navigation.Statistics");

            this.LoadStatistics = ReactiveCommand.CreateAsyncTask(_ => this.LoadStatisticsImpl());
            this.LoadStatistics.AttachExceptionHandler();
            this.LoadStatistics.AttachLoadingService(CTime2Resources.Get("Loading.Statistics"));
            this.LoadStatistics.TrackEvent("LoadStatistics");
            this.LoadStatistics.ToLoadedProperty(this, f => f.Statistics, out this._statisticsHelper);
            
            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }
        #endregion

        #region Methods
        protected override async void OnActivate()
        {
            await this.LoadStatistics.ExecuteAsyncTask();
        }
        
        private async Task<ReactiveObservableCollection<StatisticItem>> LoadStatisticsImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

            var timesByDay = TimesByDay.Create(times)
                .Where(this.IsTimeByDayForStatistic)
                .ToList();

            if (times.Count == 0 || timesByDay.Count == 0 || timesByDay.Count(f => f.Hours != TimeSpan.Zero) == 0)
            {
                await this._dialogService.ShowAsync(CTime2Resources.Get("Statistics.NoTimesBetweenStartAndEndDate"));
                return new ReactiveObservableCollection<StatisticItem>();
            }

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

            var expectedWorkTimeInMinutes = timesByDay.Count(f => f.Hours != TimeSpan.Zero) * TimeSpan.FromHours(8).TotalMinutes;
            var workTimePoolInMinutes = (int)(timesByDay.Sum(f => f.Hours.TotalMinutes) - expectedWorkTimeInMinutes);

            var timeToday = TimesByDay.Create(times).FirstOrDefault(f => f.Day.Date == DateTime.Today);
            var latestTimeToday = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault();
            var workTimeTodayToUseUpOverTimePool = TimeSpan.FromHours(8)
                - TimeSpan.FromMinutes(workTimePoolInMinutes)
                - (timeToday?.Hours ?? TimeSpan.Zero)
                + (latestTimeToday?.Duration ?? TimeSpan.Zero);
            var hadBreakAlready = timeToday?.Times.Count >= 2;
            var expectedWorkEnd = (latestTimeToday?.ClockInTime ?? DateTime.Now) 
                + (hadBreakAlready ? TimeSpan.Zero : TimeSpan.FromHours(1)) 
                + workTimeTodayToUseUpOverTimePool;
            
            return new ReactiveObservableCollection<StatisticItem>
            {
                new StatisticItem(CTime2Resources.Get("Statistics.AverageWorkTime"), averageWorkTime.TrimMilliseconds().ToString("T")),
                new StatisticItem(CTime2Resources.Get("Statistics.AverageBreakTime"), averageBreakTime.TrimMilliseconds().ToString("T")),
                new StatisticItem(CTime2Resources.Get("Statistics.AverageEnterTime"), averageEnterTime.ToDateTime().ToString("T")),
                new StatisticItem(CTime2Resources.Get("Statistics.AverageLeaveTime"), averageLeaveTime.ToDateTime().ToString("T")),
                new StatisticItem(CTime2Resources.Get("Statistics.TotalWorkDays"), totalWorkDays.ToString()),
                new StatisticItem(CTime2Resources.Get("Statistics.TotalWorkTime"), totalWorkTime.ToString(CTime2Resources.Get("Statistics.TotalWorkTimeFormat"))),
                new StatisticItem(CTime2Resources.Get("Statistics.OverTimePool"), workTimePoolInMinutes.ToString()),
                new StatisticItem(CTime2Resources.Get("Statistics.CalculatedLeaveTimeToday"), expectedWorkEnd.ToString("T")),
            };
        }

        private bool IsTimeByDayForStatistic(TimesByDay timesByDay)
        {
            if (timesByDay.Day != DateTime.Today)
                return true;

            if (timesByDay.Times.Count(f => f.ClockOutTime.HasValue) >= 2)
                return true;

            return false;
        }
        #endregion
    }
}
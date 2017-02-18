using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using System.Reactive;
using System.Text;
using CTime2.Core.Services.Sharing;
using CTime2.Views.Statistics.Details;
using UwCore.Services.Navigation;

namespace CTime2.Views.Statistics
{
    public class StatisticsViewModel : UwCoreScreen
    {
        #region Fields
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ISharingService _sharingService;
        
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;
        private bool? _includeToday;
        private readonly ObservableAsPropertyHelper<ReactiveList<StatisticItem>> _statisticsHelper;
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
        public bool? IncludeToday
        {
            get { return this._includeToday; }
            set { this.RaiseAndSetIfChanged(ref this._includeToday, value); }
        }

        public ReactiveList<StatisticItem> Statistics => this._statisticsHelper.Value;
        #endregion

        #region Commands
        public UwCoreCommand<ReactiveList<StatisticItem>> LoadStatistics { get; }
        public UwCoreCommand<Unit> Share { get; }
        #endregion

        #region Constructors
        public StatisticsViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IDialogService dialogService, INavigationService navigationService, ISharingService sharingService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(sharingService, nameof(sharingService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._dialogService = dialogService;
            this._navigationService = navigationService;
            this._sharingService = sharingService;

            this.LoadStatistics = UwCoreCommand.Create(this.LoadStatisticsImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Statistics"))
                .HandleExceptions()
                .TrackEvent("LoadStatistics");
            this.LoadStatistics.ToProperty(this, f => f.Statistics, out this._statisticsHelper);

            this.Share = UwCoreCommand.Create(this.ShareImpl)
                .HandleExceptions()
                .TrackEvent("ShareStatistics");

            this.WhenAnyValue(f => f.StartDate, f => f.EndDate)
                .Select(f => CTime2Resources.GetFormatted("Statistics.TitleFormat", this.StartDate, this.EndDate))
                .Subscribe(name => this.DisplayName = name);
            
            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.WithoutTime();
        }
        #endregion

        protected override void SaveState(IApplicationStateService applicationStateService)
        {
            base.SaveState(applicationStateService);

            applicationStateService.Set(nameof(this.StartDate), this.StartDate, ApplicationState.Temp);
            applicationStateService.Set(nameof(this.EndDate), this.EndDate, ApplicationState.Temp);
        }

        protected override void RestoreState(IApplicationStateService applicationStateService)
        {
            base.RestoreState(applicationStateService);

            var startDate = applicationStateService.Get<DateTimeOffset?>(nameof(this.StartDate), ApplicationState.Temp);
            if (startDate != null)
                this.StartDate = startDate.Value;

            var endDate = applicationStateService.Get<DateTimeOffset?>(nameof(this.EndDate), ApplicationState.Temp);
            if (endDate != null)
            {
                this.EndDate = endDate.Value;
            }
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadStatistics.ExecuteAsync();
        }

        #region Methods
        private async Task<ReactiveList<StatisticItem>> LoadStatisticsImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

            var allTimes = TimesByDay.Create(times).ToList();

            var timeToday = allTimes.FirstOrDefault(f => f.Day.Date == DateTime.Today);

            //If "IncludeToday" is NULL, we have to set it to either true or false
            //NULL is the default value when the first time the LoadStatistics command is executed
            if (this.IncludeToday.HasValue == false)
            {
                var completedTimesToday = timeToday?.Times.Count(f => f.ClockInTime != null && f.ClockOutTime != null);

                var hasAtLeastTwoCompletedTimesToday = completedTimesToday.HasValue && completedTimesToday.Value >= 2;
                var lastTimeIsCompleted = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault()?.ClockOutTime != null;

                this.IncludeToday = hasAtLeastTwoCompletedTimesToday && lastTimeIsCompleted;
            }

            var timesByDay = allTimes
                .Where(f => f.Day.Date != DateTime.Today || this.IncludeToday.Value)
                .ToList();
            
            if (times.Count == 0 || timesByDay.Count == 0 || timesByDay.Count(f => f.Hours != TimeSpan.Zero) == 0)
            {
                await this._dialogService.ShowAsync(CTime2Resources.Get("Statistics.NoTimesBetweenStartAndEndDate"));
                return new ReactiveList<StatisticItem>();
            }

            var workDayHours = this._applicationStateService.GetWorkDayHours();
            var workDayBreak = this._applicationStateService.GetWorkDayBreak();

            var totalWorkTime = TimeSpan.FromMinutes(timesByDay.Where(f => f.Hours != TimeSpan.Zero).Sum(f => f.Hours.TotalMinutes));

            var totalWorkDays = timesByDay.Count(f => f.Hours != TimeSpan.Zero);

            var averageWorkTime = TimeSpan.FromMinutes(totalWorkTime.TotalMinutes / totalWorkDays);

            var averageEnterTime = TimeSpan.FromMinutes(
                timesByDay.Where(f => f.DayStartTime != null).Sum(f => f.DayStartTime.Value.TimeOfDay.TotalMinutes) /
                timesByDay.Count(f => f.DayStartTime != null));

            var averageLeaveTime = TimeSpan.FromMinutes(
                timesByDay.Where(f => f.DayEndTime != null).Sum(f => (f.DayEndTime.Value - f.Day).TotalMinutes) /
                timesByDay.Count(f => f.DayEndTime != null));

            var averageBreakTime = averageLeaveTime - averageEnterTime - averageWorkTime;

            var expectedWorkTimeInMinutes = timesByDay.Count(f => f.Hours != TimeSpan.Zero) * workDayHours.TotalMinutes;
            var workTimePoolInMinutes = (int)Math.Round(timesByDay.Sum(f => f.Hours.TotalMinutes) - expectedWorkTimeInMinutes);

            var latestTimeToday = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault();
            var workTimeTodayToUseUpOverTimePool = workDayHours
                - TimeSpan.FromMinutes(workTimePoolInMinutes)
                - (timeToday?.Hours ?? TimeSpan.Zero)
                + (latestTimeToday?.Duration ?? TimeSpan.Zero);
            var hadBreakAlready = timeToday?.Times.Count >= 2;
            var hasExpectedWorkEnd = (latestTimeToday?.ClockInTime) != null;
            var expectedWorkEnd = (latestTimeToday?.ClockInTime ?? DateTime.Now) 
                + (hadBreakAlready ? TimeSpan.Zero : workDayBreak) 
                + workTimeTodayToUseUpOverTimePool;
            var expectedWorkEndWithoutOverTime = expectedWorkEnd + TimeSpan.FromMinutes(workTimePoolInMinutes);

            var statisticItems = new List<StatisticItem>
            {
                new StatisticItem(
                    CTime2Resources.Get("Statistics.AverageWorkTime"),
                    null,
                    averageWorkTime.TrimMilliseconds().ToString("T"),
                    timesByDay.Count > 1
                        ? () => this.ShowDetails(StatisticChartKind.WorkTime)
                        : (Action)null),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.AverageBreakTime"),
                    null,
                    averageBreakTime.TrimMilliseconds().ToString("T"),
                    timesByDay.Count > 1
                        ? () => this.ShowDetails(StatisticChartKind.BreakTime)
                        : (Action)null),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.AverageEnterTime"),
                    null,
                    averageEnterTime.ToDateTime().ToString("T"),
                    timesByDay.Count > 1
                        ? () => this.ShowDetails(StatisticChartKind.EnterAndLeaveTime)
                        : (Action)null),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.AverageLeaveTime"),
                    null,
                    averageLeaveTime.ToDateTime().ToString("T"),
                    timesByDay.Count > 1
                        ? () => this.ShowDetails(StatisticChartKind.EnterAndLeaveTime)
                        : (Action)null),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.TotalWorkDays"),
                    null,
                    totalWorkDays.ToString()),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.TotalWorkTime"),
                    null,
                    totalWorkTime.ToString(CTime2Resources.Get("Statistics.TotalWorkTimeFormat"))),

                new StatisticItem(
                    CTime2Resources.Get("Statistics.OverTimePool"),
                    CTime2Resources.Get("Statistics.OverTimePoolSubTitle"),
                    workTimePoolInMinutes.ToString(),
                    timesByDay.Count > 1
                        ? () => this.ShowDetails(StatisticChartKind.OverTime)
                        : (Action)null),

                hasExpectedWorkEnd 
                    ? new StatisticItem(
                        CTime2Resources.Get("Statistics.CalculatedLeaveTimeToday"),
                        null,
                        expectedWorkEnd.ToString("T"))
                    : null,

                hasExpectedWorkEnd
                    ? new StatisticItem(
                        CTime2Resources.Get("Statistics.CalculatedLeaveTimeToday"),
                        CTime2Resources.Get("Statistics.CalculatedLeaveTimeTodayWithoutOvertimeSubTitle"),
                        expectedWorkEndWithoutOverTime.ToString("T"))
                    : null,
            };

            return new ReactiveList<StatisticItem>(statisticItems.Where(f => f != null));
        }
        
        private Task ShareImpl()
        {
            this._sharingService.Share(this.DisplayName, package =>
            {
                var message = new StringBuilder();
                foreach (var statisticItem in this.Statistics.EmptyIfNull()) //If an error occured while loading the statistics, they might be NULL
                {
                    message.AppendLine(statisticItem.ToString());
                    message.AppendLine();
                }

                package.SetText(message.ToString());
            });

            return Task.CompletedTask;
        }

        private void ShowDetails(StatisticChartKind chartKind)
        {
            this._navigationService.Popup
                .For<DetailedStatisticViewModel>()
                .WithParam(f => f.Parameter.StartDate, this.StartDate)
                .WithParam(f => f.Parameter.EndDate, this.EndDate)
                .WithParam(f => f.Parameter.StatisticChart, chartKind)
                .WithParam(f => f.Parameter.IncludeToday, this.IncludeToday.GetValueOrDefault())
                .Navigate();
        }
        #endregion
    }
}
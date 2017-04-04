using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Statistics;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Views.Statistics.Details.BreakTime
{
    public class BreakTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IStatisticsService _statisticsService;

        private double _expectedBreakTimePerDay;
        private double _averageBreakTimePerDay;
        private ReactiveList<StatisticChartItem> _chartItems;

        public double ExpectedBreakTimePerDay
        {
            get { return this._expectedBreakTimePerDay; }
            set { this.RaiseAndSetIfChanged(ref this._expectedBreakTimePerDay, value); }
        }

        public double AverageBreakTimePerDay
        {
            get { return this._averageBreakTimePerDay; }
            set { this.RaiseAndSetIfChanged(ref this._averageBreakTimePerDay, value); }
        }

        public ReactiveList<StatisticChartItem> ChartItems
        {
            get { return this._chartItems; }
            set { this.RaiseAndSetIfChanged(ref this._chartItems, value); }
        }

        public BreakTimeViewModel(IApplicationStateService applicationStateService, IStatisticsService statisticsService, IClock clock)
            : base(clock)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(statisticsService, nameof(statisticsService));

            this._applicationStateService = applicationStateService;
            this._statisticsService = statisticsService;

            this.DisplayName = CTime2Resources.Get("Navigation.BreakTime");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var items = timesByDay
                .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = (f.DayEndTime.Value - f.DayStartTime.Value).TotalMinutes - f.Hours.TotalMinutes
                })
                .ToList();
            this.EnsureAllDatesAreThere(items, 0);

            this.ExpectedBreakTimePerDay = this._applicationStateService.GetWorkDayBreak().TotalMinutes;
            this.AverageBreakTimePerDay = this._statisticsService.CalculateAverageBreakTime(timesByDay, onlyWorkDays:true, onlyDaysWithBreak:false).TotalMinutes;
            this.ChartItems = new ReactiveList<StatisticChartItem>(items.OrderBy(f => f.Date));

            return Task.CompletedTask;
        }
    }
}
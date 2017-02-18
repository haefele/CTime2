using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Core.Services.ApplicationState;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Statistics.Details.BreakTime
{
    public class BreakTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private readonly IApplicationStateService _applicationStateService;

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

        public BreakTimeViewModel(IApplicationStateService applicationStateService)
        {
            this._applicationStateService = applicationStateService;

            this.DisplayName = CTime2Resources.Get("BreakTimeChart.Title");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var timesForBreakTime = timesByDay
                .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                .ToList();

            var items = timesForBreakTime
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = (f.DayEndTime.Value - f.DayStartTime.Value).TotalMinutes - f.Hours.TotalMinutes
                })
                .ToList();
            this.EnsureAllDatesAreThere(items, 0);

            this.ExpectedBreakTimePerDay = this._applicationStateService.GetWorkDayBreak().TotalHours;
            this.AverageBreakTimePerDay = timesForBreakTime.Sum(f => (f.DayEndTime.Value - f.DayStartTime.Value - f.Hours).TotalMinutes) / timesForBreakTime.Count;
            this.ChartItems = new ReactiveList<StatisticChartItem>(items.OrderBy(f => f.Date));

            return Task.CompletedTask;
        }
    }
}
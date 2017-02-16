using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;

namespace CTime2.Views.Statistics.Details.BreakTime
{
    public class BreakTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private ReactiveList<StatisticChartItem> _chartItems;

        public ReactiveList<StatisticChartItem> ChartItems
        {
            get { return this._chartItems; }
            set { this.RaiseAndSetIfChanged(ref this._chartItems, value); }
        }

        public BreakTimeViewModel()
        {
            this.DisplayName = CTime2Resources.Get("BreakTimeChart.Title");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var items = timesByDay
                .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = f.DayEndTime.Value.TotalMinutes - f.DayStartTime.Value.TotalMinutes - f.Hours.TotalMinutes
                })
                .ToList();
            this.EnsureAllDatesAreThere(items, 0);

            this.ChartItems = new ReactiveList<StatisticChartItem>(items);

            return Task.CompletedTask;
        }
    }
}
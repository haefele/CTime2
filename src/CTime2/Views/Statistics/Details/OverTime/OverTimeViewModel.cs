using System.Collections.Generic;
using System.Threading.Tasks;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using System.Linq;
using System;
using CTime2.Core.Data;
using UwCore.Services.ApplicationState;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Statistics;
using ReactiveUI;
using UwCore.Common;
using UwCore.Services.Clock;
using DynamicData.Binding;

namespace CTime2.Views.Statistics.Details.OverTime
{
    public class OverTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IStatisticsService _statisticsService;

        private ObservableCollectionExtended<StatisticChartItem> _chartItems;
        private double _averageOverTimePerDay;

        public double AverageOverTimePerDay
        {
            get { return this._averageOverTimePerDay; }
            set { this.RaiseAndSetIfChanged(ref this._averageOverTimePerDay, value); }
        }

        public ObservableCollectionExtended<StatisticChartItem> ChartItems
        {
            get { return this._chartItems; }
            set { this.RaiseAndSetIfChanged(ref this._chartItems, value); }
        }

        public OverTimeViewModel(IApplicationStateService applicationStateService, IStatisticsService statisticsService, IClock clock)
            : base(clock)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(statisticsService, nameof(statisticsService));

            this._applicationStateService = applicationStateService;
            this._statisticsService = statisticsService;

            this.DisplayName = CTime2Resources.Get("Navigation.OverTime");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var result = new List<StatisticChartItem>();
            var workDays = this._applicationStateService.GetWorkDays();
            
            foreach (var time in timesByDay)
            {
                var previousDay = result.LastOrDefault();

                var change = time.Hours == TimeSpan.Zero //Use 0 and not -480 if we have no times at one day (Weekend)
                    ? 0
                    : workDays.Contains(time.Day.DayOfWeek)
                        ? (time.Hours - this._applicationStateService.GetWorkDayHours()).TotalMinutes
                        : time.Hours.TotalMinutes;
                
                result.Add(new StatisticChartItem
                {
                    Date = time.Day,
                    Value = (previousDay?.Value ?? 0) + change
                });
            }
            this.EnsureAllDatesAreThere(result, valueForFilledDates: result.Last().Value);

            this.AverageOverTimePerDay = this._statisticsService.CalculateAverageOverTime(timesByDay, onlyWorkDays: false).TotalMinutes;
            this.ChartItems = new ObservableCollectionExtended<StatisticChartItem>(result.OrderBy(f => f.Date));

            return Task.CompletedTask;
        }
    }
}
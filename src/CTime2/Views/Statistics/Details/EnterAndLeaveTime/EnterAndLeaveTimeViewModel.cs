﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.Statistics;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using DynamicData.Binding;
using ReactiveUI;
using UwCore.Common;
using UwCore.Services.Clock;

namespace CTime2.Views.Statistics.Details.EnterAndLeaveTime
{
    public class EnterAndLeaveTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private readonly IStatisticsService _statisticsService;

        private double _averageBeginTime;
        private ObservableCollectionExtended<StatisticChartItem> _beginChartItems;
        private double _averageEndTime;
        private ObservableCollectionExtended<StatisticChartItem> _endChartItems;

        public double AverageBeginTime
        {
            get { return this._averageBeginTime; }
            set { this.RaiseAndSetIfChanged(ref this._averageBeginTime, value); }
        }

        public ObservableCollectionExtended<StatisticChartItem> BeginChartItems
        {
            get { return this._beginChartItems; }
            set { this.RaiseAndSetIfChanged(ref this._beginChartItems, value); }
        }

        public double AverageEndTime
        {
            get { return this._averageEndTime; }
            set { this.RaiseAndSetIfChanged(ref this._averageEndTime, value); }
        }

        public ObservableCollectionExtended<StatisticChartItem> EndChartItems
        {
            get { return this._endChartItems; }
            set { this.RaiseAndSetIfChanged(ref this._endChartItems, value); }
        }

        public EnterAndLeaveTimeViewModel(IStatisticsService statisticsService, IClock clock)
            : base(clock)
        {
            Guard.NotNull(statisticsService, nameof(statisticsService));

            this._statisticsService = statisticsService;

            this.DisplayName = CTime2Resources.Get("Navigation.EnterAndLeaveTime");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var begin = timesByDay
                .Where(f => f.DayStartTime != null)
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = f.DayStartTime.Value.TimeOfDay.TotalHours
                })
                .ToList();
            this.EnsureAllDatesAreThere(begin, 0);

            this.AverageBeginTime = this._statisticsService.CalculateAverageEnterTime(timesByDay, onlyWorkDays:true).TotalHours;
            this.BeginChartItems = new ObservableCollectionExtended<StatisticChartItem>(begin.OrderBy(f => f.Date));

            var end = timesByDay
                .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = (f.DayEndTime.Value.Date - f.DayStartTime.Value.Date).TotalHours + f.DayEndTime.Value.TimeOfDay.TotalHours
                })
                .ToList();
            this.EnsureAllDatesAreThere(end, 0);

            this.AverageEndTime = this._statisticsService.CalculateAverageLeaveTime(timesByDay, onlyWorkDays:true).TotalHours;
            this.EndChartItems = new ObservableCollectionExtended<StatisticChartItem>(end.OrderBy(f => f.Date));

            return Task.CompletedTask;
        }
    }
}
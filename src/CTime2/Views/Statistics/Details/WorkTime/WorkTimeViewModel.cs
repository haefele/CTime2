﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Statistics;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using DynamicData.Binding;
using ReactiveUI;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Views.Statistics.Details.WorkTime
{
    public class WorkTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IStatisticsService _statisticsService;

        private double _expectedHoursPerDay;
        private double _averageHoursPerDay;
        private ObservableCollectionExtended<StatisticChartItem> _chartItems;

        public double ExpectedHoursPerDay
        {
            get { return this._expectedHoursPerDay; }
            set { this.RaiseAndSetIfChanged(ref this._expectedHoursPerDay, value); }
        }

        public double AverageHoursPerDay
        {
            get { return this._averageHoursPerDay; }
            set { this.RaiseAndSetIfChanged(ref this._averageHoursPerDay, value); }
        }

        public ObservableCollectionExtended<StatisticChartItem> ChartItems
        {
            get { return this._chartItems; }
            set { this.RaiseAndSetIfChanged(ref this._chartItems, value); }
        }

        public WorkTimeViewModel(IApplicationStateService applicationStateService, IStatisticsService statisticsService, IClock clock)
            : base(clock)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(statisticsService, nameof(statisticsService));

            this._applicationStateService = applicationStateService;
            this._statisticsService = statisticsService;

            this.DisplayName = CTime2Resources.Get("Navigation.WorkTime");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            var items = timesByDay
                .Select(f => new StatisticChartItem
                {
                    Date = f.Day,
                    Value = f.Hours.TotalHours
                })
                .ToList();
            this.EnsureAllDatesAreThere(items, 0);

            this.ExpectedHoursPerDay = this._applicationStateService.GetWorkDayHours().TotalHours;
            this.AverageHoursPerDay = this._statisticsService.CalculateAverageWorkTime(timesByDay, onlyWorkDays:true).TotalHours;
            this.ChartItems = new ObservableCollectionExtended<StatisticChartItem>(items.OrderBy(f => f.Date));

            return Task.CompletedTask;
        }
    }
}
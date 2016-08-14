using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Statistics
{
    public class StatisticChartItem
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public enum StatisticChartKind
    {
        WorkTime,
        BreakTime,
        EnterTime,
        LeaveTime,
        OverTime,
    }

    public class DetailedStatisticViewModel : ReactiveScreen
    {
        #region Fields
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<StatisticChartItem>> _chartItemsHelper;
        #endregion

        #region Properties
        public ReactiveObservableCollection<StatisticChartItem> ChartItems => this._chartItemsHelper.Value;
        #endregion

        #region Commands
        public ReactiveCommand<ReactiveObservableCollection<StatisticChartItem>> LoadChart { get; }
        #endregion

        #region Parameters
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public StatisticChartKind StatisticChart { get; set; }
        #endregion

        public DetailedStatisticViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            this.LoadChart = ReactiveCommand.CreateAsyncTask(_ => this.LoadChartImpl());
            this.LoadChart.AttachExceptionHandler();
            this.LoadChart.AttachLoadingService("Lade Diagram");
            this.LoadChart.ToLoadedProperty(this, f => f.ChartItems, out this._chartItemsHelper);
        }

        protected override async void OnActivate()
        {
            await this.LoadChart.ExecuteAsyncTask();
        }

        private async Task<ReactiveObservableCollection<StatisticChartItem>> LoadChartImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

            var timesByDay = TimesByDay.Create(times)
                .Where(TimesByDay.IsForStatistic)
                .OrderBy(f => f.Day)
                .ToList();
            
            var result = new ReactiveObservableCollection<StatisticChartItem>(this.GetChartItems(timesByDay));

            this.EnsureAllDatesAreThere(result);

            return result;
        }
        
        private void EnsureAllDatesAreThere(ReactiveObservableCollection<StatisticChartItem> result)
        {
            var endDate = new DateTimeOffset(result.Max(f => f.Date));

            for (var date = this.StartDate; date <= endDate; date = date.AddDays(1))
            {
                var dateIsMissing = result.Any(f => f.Date == date) == false;
                if (dateIsMissing)
                {
                    result.Add(new StatisticChartItem
                    {
                        Date = date.Date,
                        Value = 0
                    });
                }
            }
        }

        private IEnumerable<StatisticChartItem> GetChartItems(IList<TimesByDay> times)
        {
            switch (this.StatisticChart)
            {
                case StatisticChartKind.WorkTime:
                    return times
                        .Select(f => new StatisticChartItem
                        {
                            Date = f.Day,
                            Value = f.Hours.TotalHours
                        });

                case StatisticChartKind.BreakTime:
                    return times
                        .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                        .Select(f => new StatisticChartItem
                        {
                            Date = f.Day,
                            Value = f.DayEndTime.Value.TotalMinutes - f.DayStartTime.Value.TotalMinutes - f.Hours.TotalMinutes
                        });

                case StatisticChartKind.EnterTime:
                    return times
                        .Select(f => new StatisticChartItem
                        {
                            Date = f.Day,
                            Value = f.DayStartTime?.TotalHours ?? 0
                        });

                case StatisticChartKind.LeaveTime:
                    return times
                        .Select(f => new StatisticChartItem
                        {
                            Date = f.Day,
                            Value = f.DayEndTime?.TotalHours ?? 0
                        });

                case StatisticChartKind.OverTime:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

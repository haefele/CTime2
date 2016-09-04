using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Navigation;

namespace CTime2.Views.Statistics
{
    public class DetailedStatisticViewModel : ReactiveScreen
    {
        #region Fields
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<StatisticChartItem>[]> _chartItemsHelper;
        #endregion

        #region Properties
        public ReactiveObservableCollection<StatisticChartItem>[] ChartItems => this._chartItemsHelper.Value;
        #endregion

        #region Commands
        public ReactiveCommand<ReactiveObservableCollection<StatisticChartItem>[]> LoadChart { get; }
        public ReactiveCommand<Unit> GoToMyTimesCommand { get; set; }
        #endregion

        #region Parameters
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public StatisticChartKind StatisticChart { get; set; }
        #endregion

        public DetailedStatisticViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, INavigationService navigationService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;

            this.LoadChart = ReactiveCommand.CreateAsyncTask(_ => this.LoadChartImpl());
            this.LoadChart.AttachExceptionHandler();
            this.LoadChart.AttachLoadingService(CTime2Resources.Get("Loading.LoadCharts"));
            this.LoadChart.ToLoadedProperty(this, f => f.ChartItems, out this._chartItemsHelper);

            this.GoToMyTimesCommand = ReactiveCommand.CreateAsyncTask(_ => this.GoToMyTimes());
            this.GoToMyTimesCommand.AttachExceptionHandler();
        }

        public void GoToMyTimesForStatisticChartItem(StatisticChartItem chartItem)
        {
            this._navigationService
                .For<YourTimesViewModel>()
                .WithParam(f => f.StartDate, new DateTimeOffset(chartItem.Date))
                .WithParam(f => f.EndDate, new DateTimeOffset(chartItem.Date))
                .Navigate();
        }

        protected override async void OnActivate()
        {
            await this.LoadChart.ExecuteAsyncTask();
        }

        private async Task<ReactiveObservableCollection<StatisticChartItem>[]> LoadChartImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

            var timesByDay = TimesByDay.Create(times)
                .Where(TimesByDay.IsForStatistic)
                .OrderBy(f => f.Day)
                .ToList();

            return this.GetChartItems(timesByDay)
                .Select(f => new ReactiveObservableCollection<StatisticChartItem>(f))
                .Select(f => { this.EnsureAllDatesAreThere(f, valueForFilledDates:0); return f; })
                .ToArray();
        }

        private Task GoToMyTimes()
        {
            this._navigationService
                .For<YourTimesViewModel>()
                .WithParam(f => f.StartDate, this.StartDate)
                .WithParam(f => f.EndDate, this.EndDate)
                .Navigate();

            return Task.CompletedTask;
        }

        private void EnsureAllDatesAreThere(ICollection<StatisticChartItem> result, double valueForFilledDates)
        {
            var endDate = new DateTimeOffset(result.Max(f => f.Date));
            if (this.EndDate >= DateTimeOffset.Now)
            {
                endDate = DateTimeOffset.Now;
            }

            for (var date = this.StartDate; date <= endDate; date = date.AddDays(1))
            {
                var dateIsMissing = result.Any(f => f.Date == date) == false;
                if (dateIsMissing)
                {
                    result.Add(new StatisticChartItem
                    {
                        Date = date.Date,
                        Value = valueForFilledDates
                    });
                }
            }
        }

        private IEnumerable<StatisticChartItem>[] GetChartItems(IList<TimesByDay> times)
        {
            switch (this.StatisticChart)
            {
                case StatisticChartKind.WorkTime:
                    return new[]
                    {
                        times.Select(f => new StatisticChartItem
                        {
                            Date = f.Day,
                            Value = f.Hours.TotalHours
                        })
                    };

                case StatisticChartKind.BreakTime:
                    return new[]
                    {
                        times.Where(f => f.DayStartTime != null && f.DayEndTime != null)
                             .Select(f => new StatisticChartItem
                             {
                                 Date = f.Day,
                                 Value = f.DayEndTime.Value.TotalMinutes - f.DayStartTime.Value.TotalMinutes - f.Hours.TotalMinutes
                             })
                    };

                case StatisticChartKind.EnterAndLeaveTime:
                    return new[]
                    {
                        times.Where(f => f.DayStartTime != null)
                             .Select(f => new StatisticChartItem
                             {
                                 Date = f.Day,
                                 Value = f.DayStartTime.Value.TotalHours
                             }),
                        times.Where(f => f.DayEndTime != null)
                             .Select(f => new StatisticChartItem
                             {
                                 Date = f.Day,
                                 Value = f.DayEndTime.Value.TotalHours
                             })
                    };

                case StatisticChartKind.OverTime:
                    var result = new List<StatisticChartItem>();

                    foreach (var time in times)
                    {
                        var previousDay = result.LastOrDefault();

                        var change = time.DayStartTime == null && time.DayEndTime == null //Use 0 and not -480 if we have no times at one day (Weekend)
                            ? 0
                            : (time.Hours - TimeSpan.FromHours(8)).TotalMinutes;

                        result.Add(new StatisticChartItem
                        {
                            Date = time.Day,
                            Value = (previousDay?.Value ?? 0) + change
                        });
                    }

                    this.EnsureAllDatesAreThere(result, valueForFilledDates:result.Last().Value);

                    return new[]
                    {
                        result
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

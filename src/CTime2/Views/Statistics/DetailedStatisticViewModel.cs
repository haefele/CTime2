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
    public class ChartItem
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public enum ChartKind
    {
        WorkTime
    }

    public class DetailedStatisticViewModel : ReactiveScreen
    {
        #region Fields
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<ChartItem>> _chartItemsHelper;
        #endregion

        #region Properties
        public ReactiveObservableCollection<ChartItem> ChartItems => this._chartItemsHelper.Value;
        #endregion

        #region Commands
        public ReactiveCommand<ReactiveObservableCollection<ChartItem>> LoadChart { get; }
        #endregion

        #region Parameters
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public ChartKind Chart { get; set; }
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
            this.DisplayName = this.GetDisplayName();
            await this.LoadChart.ExecuteAsyncTask();
        }

        private async Task<ReactiveObservableCollection<ChartItem>> LoadChartImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

            var timesByDay = TimesByDay.Create(times)
                .Where(TimesByDay.IsForStatistic)
                .OrderBy(f => f.Day)
                .ToList();
            
            return new ReactiveObservableCollection<ChartItem>(timesByDay.Select(f => new ChartItem
            {
                Date = f.Day,
                Value = f.Hours.TotalHours
            }));
        }

        private string GetDisplayName()
        {
            switch (this.Chart)
            {
                case ChartKind.WorkTime:
                    return "Arbeitsdauer";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

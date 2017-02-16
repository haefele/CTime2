using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTime2.Views.YourTimes;
using ReactiveUI;
using UwCore;

namespace CTime2.Views.Statistics.Details
{
    public abstract class DetailedStatisticDiagramViewModelBase : UwCoreScreen
    {
        private double _zoomFactor;
        private bool _showLabels;

        public double ZoomFactor
        {
            get { return this._zoomFactor; }
            set { this.RaiseAndSetIfChanged(ref this._zoomFactor, value); }
        }

        public bool ShowLabels
        {
            get { return this._showLabels; }
            set { this.RaiseAndSetIfChanged(ref this._showLabels, value); }
        }

        public DetailedStatisticDiagramViewModelBase()
        {
            this.ZoomFactor = 1;
            this.ShowLabels = true;
        }

        public abstract Task LoadAsync(List<TimesByDay> timesByDay);
        
        protected void EnsureAllDatesAreThere(ICollection<StatisticChartItem> result, double valueForFilledDates)
        {
            var parent = (DetailedStatisticViewModel)this.Parent;

            var endDate = new DateTimeOffset(result.Max(f => f.Date));
            if (parent.Parameter.EndDate >= DateTimeOffset.Now)
            {
                endDate = DateTimeOffset.Now;
            }

            for (var date = parent.Parameter.StartDate; date <= endDate; date = date.AddDays(1))
            {
                var dateIsMissing = result.Any(f => f.Date.Date == date.Date) == false; //Ignore time-zone when checking if a date is missing
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
    }
}
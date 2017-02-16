using System.Collections.Generic;
using System.Threading.Tasks;
using CTime2.Strings;
using CTime2.Views.YourTimes;

namespace CTime2.Views.Statistics.Details.EnterAndLeaveTime
{
    public class EnterAndLeaveTimeViewModel : DetailedStatisticDiagramViewModelBase
    {
        public EnterAndLeaveTimeViewModel()
        {
            this.DisplayName = CTime2Resources.Get("EnterAndLeaveTimeChart.Title");
        }

        public override Task LoadAsync(List<TimesByDay> timesByDay)
        {
            //times.Where(f => f.DayStartTime != null)
            //                 .Select(f => new StatisticChartItem
            //                 {
            //                     Date = f.Day,
            //                     Value = f.DayStartTime.Value.TotalHours
            //                 }),
            //            times.Where(f => f.DayEndTime != null)
            //                 .Select(f => new StatisticChartItem
            //                 {
            //                     Date = f.Day,
            //                     Value = f.DayEndTime.Value.TotalHours
            //                 })

            throw new System.NotImplementedException();
        }
    }
}
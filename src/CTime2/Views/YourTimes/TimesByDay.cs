using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using CTime2.Core.Data;

namespace CTime2.Views.YourTimes
{
    public class TimesByDay
    {
        public DateTime Day { get; }
        public TimeSpan Hours { get; }
        public BindableCollection<TimeForGrouping> Times { get; }
        public TimeSpan? DayStartTime { get; }
        public TimeSpan? DayEndTime { get; }

        private TimesByDay(DateTime day, BindableCollection<TimeForGrouping> times)
        {
            this.Day = day;
            this.Times = times;
            this.Hours = times.Select(f => f.Time.Hours).FirstOrDefault();

            var clockInTimes = times.Where(f => f.ClockInTime != null).Select(f => f.ClockInTime.Value.TimeOfDay).ToList();
            this.DayStartTime = clockInTimes.Any() ? clockInTimes.Min() : (TimeSpan?)null;

            var clockOutTimes = times.Where(f => f.ClockOutTime != null).Select(f => f.ClockOutTime.Value.TimeOfDay).ToList();
            this.DayEndTime = clockOutTimes.Any() ? clockOutTimes.Max() : (TimeSpan?)null;
        }

        public static IEnumerable<TimesByDay> Create(IEnumerable<Time> times)
        {
            var result =
                from time in times
                orderby time.Day descending
                group time by time.Day into g
                select new TimesByDay(g.Key, new BindableCollection<TimeForGrouping>(g.OrderByDescending(f => f.ClockInTime).Select(f => new TimeForGrouping(f))));

            return result;
        }

        public static bool IsForStatistic(TimesByDay timesByDay)
        {
            if (timesByDay.Day != DateTime.Today)
                return true;

            if (timesByDay.Times.Count(f => f.ClockOutTime.HasValue) >= 2)
                return true;

            return false;
        }
    }
}
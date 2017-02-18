using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CTime2.Core.Data;

namespace CTime2.Views.YourTimes
{
    public class TimesByDay
    {
        public DateTime Day { get; }
        public TimeSpan Hours { get; }
        public BindableCollection<TimeForGrouping> Times { get; }
        public DateTime? DayStartTime { get; }
        public DateTime? DayEndTime { get; }

        private TimesByDay(DateTime day, BindableCollection<TimeForGrouping> times)
        {
            this.Day = day;
            this.Times = times;
            this.Hours = times.Select(f => f.Time.Hours).FirstOrDefault();

            var clockInTimes = times.Where(f => f.ClockInTime != null).Select(f => f.ClockInTime.Value).ToList();
            this.DayStartTime = clockInTimes.Any() ? clockInTimes.Min() : (DateTime?)null;

            var clockOutTimes = times.Where(f => f.ClockOutTime != null).Select(f => f.ClockOutTime.Value).ToList();
            this.DayEndTime = clockOutTimes.Any() ? clockOutTimes.Max(): (DateTime?)null;
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

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"{this.Day:D} ({this.Hours:g}):");

            foreach (var time in this.Times)
            {
                result.AppendLine(time.ToString());
            }

            return result.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}
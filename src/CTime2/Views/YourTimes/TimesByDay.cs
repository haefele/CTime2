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
        public TimeSpan? Hours { get; }
        public BindableCollection<TimeForGrouping> Times { get; }

        private TimesByDay(DateTime day, BindableCollection<TimeForGrouping> times)
        {
            this.Day = day;
            this.Times = times;
            this.Hours = times.Select(f => f.Time.Hours).FirstOrDefault();
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

        #region Internal
        public class TimeForGrouping
        {
            public Time Time { get; }

            public DateTime? ClockInTime { get; }
            public DateTime? ClockOutTime { get; }
            public TimeSpan? Duration { get; }

            public TimeForGrouping(Time time)
            {
                this.Time = time;

                this.ClockInTime = time.ClockInTime;
                this.ClockOutTime = time.ClockOutTime;

                if (this.ClockInTime != null && this.ClockOutTime != null)
                {
                    this.Duration = this.ClockOutTime.Value - this.ClockInTime.Value;
                }
            }
        }
        #endregion
    }
}
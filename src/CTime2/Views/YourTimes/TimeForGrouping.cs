using System;
using CTime2.Core.Data;

namespace CTime2.Views.YourTimes
{
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
}
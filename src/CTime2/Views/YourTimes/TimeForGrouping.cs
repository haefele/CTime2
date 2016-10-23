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

        public override string ToString()
        {
            string clockInTime = this.ClockInTime != null
                ? $"{this.ClockInTime.Value:t}"
                : "?";
            string clockOutTime = this.ClockOutTime != null
                ? $"{this.ClockOutTime.Value:t}"
                : "?";
            string duration = this.Duration != null
                ? $" ({this.Duration.Value:g})"
                : string.Empty;

            return $"{clockInTime} - {clockOutTime}{duration}";
        }
    }
}
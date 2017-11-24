using System;

namespace CTime2.Core.Data
{
    public class TimeForGrouping
    {
        public Time Time { get; }

        public DateTime? ClockInTime { get; }
        public DateTime? ClockOutTime { get; }
        public TimeSpan? Duration { get; }
        public string StateDescription { get; }

        public TimeForGrouping(Time time)
        {
            this.Time = time;

            this.ClockInTime = time.ClockInTime;
            this.ClockOutTime = time.ClockOutTime;
            this.StateDescription = time.StateDescription;

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
            string state = string.IsNullOrWhiteSpace(this.StateDescription) == false
                ? $" ({this.StateDescription})"
                : string.Empty;

            return $"{clockInTime} - {clockOutTime}{duration}{state}";
        }
    }
}
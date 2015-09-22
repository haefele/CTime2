using System;

namespace CTime2.Core.Data
{
    public class Time
    {
        public DateTime Day { get; set; }
        public TimeSpan Hours { get; set; }
        public TimeState? State { get; set; }
        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
    }
}
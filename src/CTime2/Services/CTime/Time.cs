using System;
using System.Globalization;

namespace CTime2.Services.CTime
{
    public class Time
    {
        public DateTime Day { get; set; }
        public TimeSpan Hours { get; set; }
        public TimeState? State { get; set; }
    }

    public enum TimeState
    {
        Entered = 1,
        Left = 2,
    }
}
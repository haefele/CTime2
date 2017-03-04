using System;

namespace CTime2.Core.Services.Statistics
{
    public class CurrentTime
    {
        public CurrentTime(TimeSpan workTime, TimeSpan? overTime, TimeSpan? breakTime, DateTime? preferredBreakTimeEnd, bool isStillRunning)
        {
            this.WorkTime = workTime;
            this.OverTime = overTime;
            this.BreakTime = breakTime;
            this.PreferredBreakTimeEnd = preferredBreakTimeEnd;
            this.IsStillRunning = isStillRunning;
        }

        public TimeSpan WorkTime { get; }
        public TimeSpan? OverTime { get; }
        public TimeSpan? BreakTime { get; }
        public DateTime? PreferredBreakTimeEnd { get; }
        public bool IsStillRunning { get; }
    }
}
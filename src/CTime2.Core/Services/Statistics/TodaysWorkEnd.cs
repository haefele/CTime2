using System;

namespace CTime2.Core.Services.Statistics
{
    public class TodaysWorkEnd
    {
        public TodaysWorkEnd(DateTime withOvertime, DateTime withoutOvertime)
        {
            this.WithOvertime = withOvertime;
            this.WithoutOvertime = withoutOvertime;
        }

        public DateTime WithOvertime { get; }
        public DateTime WithoutOvertime { get; }
    }
}
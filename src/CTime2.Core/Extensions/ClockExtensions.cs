using System;
using UwCore.Services.Clock;

namespace CTime2.Core.Extensions
{
    public static class ClockExtensions
    {
        public static DateTime Today(this IClock self)
        {
            return self.Now().Date;
        }
    }
}
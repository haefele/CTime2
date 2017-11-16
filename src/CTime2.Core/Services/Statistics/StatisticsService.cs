using System;
using System.Collections.Generic;
using System.Linq;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Services.ApplicationState;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Core.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IClock _clock;

        public StatisticsService(IApplicationStateService applicationStateService, IClock clock)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(clock, nameof(clock));

            this._applicationStateService = applicationStateService;
            this._clock = clock;
        }

        public bool ShouldIncludeToday(List<TimesByDay> times)
        {
            var timeToday = times.FirstOrDefault(f => f.Day.Date == this._clock.Today());

            var completedTimesToday = timeToday?.Times.Count(f => f.ClockInTime != null && f.ClockOutTime != null);

            var hasAtLeastTwoCompletedTimesToday = completedTimesToday.HasValue && completedTimesToday.Value >= 2;
            var lastTimeIsCompleted = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault()?.ClockOutTime != null;

            return hasAtLeastTwoCompletedTimesToday && lastTimeIsCompleted;
        }

        public TimeSpan CalculateAverageWorkTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var sum = times.Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays)).Sum(f => f.Hours.TotalMinutes);
            var count = times.Count(f => f.Hours != TimeSpan.Zero && this.FilterOnlyWorkDays(f, true));

            if (count == 0)
                count = 1;
            
            return TimeSpan.FromMinutes(sum / count);
        }

        public TimeSpan CalculateTotalWorkTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var minutes = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Sum(f => f.Hours.TotalMinutes);

            return TimeSpan.FromMinutes(minutes);
        }

        public int CalculateWorkDayCount(List<TimesByDay> times, bool onlyWorkDays)
        {
            return times
                .Where(f => f.Hours != TimeSpan.Zero)
                .Count(f => this.FilterOnlyWorkDays(f, onlyWorkDays));
        }

        public TimeSpan CalculateAverageEnterTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var sum = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Where(f => f.DayStartTime != null)
                .Sum(f => f.DayStartTime.Value.TimeOfDay.TotalMinutes);

            var count = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Count(f => f.DayStartTime != null);

            if (count == 0)
                count = 1;

            return TimeSpan.FromMinutes(sum / count);
        }

        public TimeSpan CalculateAverageLeaveTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var sum = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Where(f => f.DayStartTime != null && f.DayEndTime != null)
                .Sum(f => (f.DayEndTime.Value - f.DayStartTime.Value.Date).TotalMinutes);

            var count = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Count(f => f.DayStartTime != null && f.DayEndTime != null);

            if (count == 0)
                count = 1;

            return TimeSpan.FromMinutes(sum / count);
        }

        public TimeSpan CalculateAverageBreakTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var breakTimes = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Select(f => this.GetBreakDurationOnDay(f.Times.OrderBy(d => d.ClockInTime).ToList()))
                .Where(f => f != null)
                .Select(f => f.Value)
                .ToList();

            if (breakTimes.Count == 0)
                return TimeSpan.Zero;

            var averageBreakTime = breakTimes.Sum(f => f.TotalMinutes) / breakTimes.Count;
            return TimeSpan.FromMinutes(averageBreakTime);
        }

        private TimeSpan? GetBreakDurationOnDay(List<TimeForGrouping> times)
        {
            if (times.Count < 2)
                return null;

            var breakTimeBegin = this._applicationStateService.GetBreakTimeBegin();
            var breakTimeEnd = this._applicationStateService.GetBreakTimeEnd();

            TimeSpan breakTimeSum = TimeSpan.Zero;

            for (int i = 0; i < times.Count; i++)
            {
                var currentTime = times[i];
                var nextTime = times.Count > i + 1 ? times[i + 1] : null;

                if (nextTime == null)
                    continue;

                if (currentTime.ClockOutTime == null || nextTime.ClockInTime == null)
                    continue;

                if (currentTime.ClockOutTime.Value.TimeOfDay >= breakTimeBegin &&
                    currentTime.ClockOutTime.Value.TimeOfDay <= breakTimeEnd &&
                    nextTime.ClockInTime.Value.TimeOfDay <= breakTimeEnd &&
                    currentTime.ClockOutTime.Value.TimeOfDay <= nextTime.ClockInTime.Value.TimeOfDay)
                {
                    breakTimeSum += nextTime.ClockInTime.Value.TimeOfDay - currentTime.ClockOutTime.Value.TimeOfDay;
                }
            }

            return breakTimeSum == TimeSpan.Zero 
                ? (TimeSpan?) null 
                : breakTimeSum;
        }

        public TimeSpan CalculateOverTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var dayCount = this.CalculateWorkDayCount(times, onlyWorkDays: true);
            var expectedWorkTime = TimeSpan.FromMinutes(this._applicationStateService.GetWorkDayHours().TotalMinutes * dayCount);
            var actualWorkTime = TimeSpan.FromMinutes(times.Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays)).Sum(f => f.Hours.TotalMinutes));

            return actualWorkTime - expectedWorkTime;
        }

        public TodaysWorkEnd CalculateTodaysWorkEnd(TimesByDay timeToday, List<TimesByDay> times, bool onlyWorkDays)
        {
            var workDayHours = this._applicationStateService.GetWorkDayHours();
            var workDayBreak = this._applicationStateService.GetWorkDayBreak();
            
            var overtime = this.CalculateOverTime(times, onlyWorkDays);

            var latestTimeToday = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault();

            if (latestTimeToday?.ClockInTime == null)
                return null;

            var workTimeTodayToUseUpOverTimePool = workDayHours
                                                   - overtime
                                                   - timeToday.Hours
                                                   + (latestTimeToday.Duration ?? TimeSpan.Zero);
            var hadBreakAlready = timeToday?.Times.Count >= 2;

            var expectedWorkEnd = (latestTimeToday?.ClockInTime ?? this._clock.Now().DateTime)
                                  + (hadBreakAlready ? TimeSpan.Zero : workDayBreak)
                                  + workTimeTodayToUseUpOverTimePool;
            var expectedWorkEndWithoutOverTime = expectedWorkEnd + overtime;
            
            return new TodaysWorkEnd(expectedWorkEnd, expectedWorkEndWithoutOverTime);
        }

        public TimeSpan CalculateAverageOverTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var overTime = this.CalculateOverTime(times, onlyWorkDays);
            var workDays = this.CalculateWorkDayCount(times, onlyWorkDays:true);

            return TimeSpan.FromMinutes(overTime.TotalMinutes / workDays);
        }

        public CurrentTime CalculateCurrentTime(Time currentTime)
        {
            if (currentTime == null)
                return new CurrentTime(TimeSpan.Zero, null, null, false);

            var now = this._clock.Now().DateTime;

            //Only take the timeToday if the time is either
            // - from today
            // - or from yesterday, but still checked-in
            var timeToday = currentTime.Day == now.Date || currentTime.State.IsEntered()
                ? currentTime.Hours
                : TimeSpan.Zero;

            if (currentTime.State.IsEntered())
                timeToday += now - currentTime.ClockInTime.Value;

            CurrentBreak breakTime = null;
         
            if (currentTime.Day == now.Date &&
                currentTime.State.IsLeft() && 
                currentTime.ClockOutTime.HasValue &&
                currentTime.ClockOutTime.Value.TimeOfDay >= this._applicationStateService.GetBreakTimeBegin() &&
                currentTime.ClockOutTime.Value.TimeOfDay <= this._applicationStateService.GetBreakTimeEnd() &&
                now.TimeOfDay >= this._applicationStateService.GetBreakTimeBegin() &&
                now.TimeOfDay <= this._applicationStateService.GetBreakTimeEnd())
            {
                breakTime = new CurrentBreak(now - currentTime.ClockOutTime.Value, currentTime.ClockOutTime.Value.Add(this._applicationStateService.GetWorkDayBreak()));
            }

            TimeSpan? overtime = null;

            if (timeToday - this._applicationStateService.GetWorkDayHours() > TimeSpan.FromSeconds(1))
            {
                overtime = timeToday - this._applicationStateService.GetWorkDayHours();
                timeToday = this._applicationStateService.GetWorkDayHours();
            }
            
            return new CurrentTime(timeToday, overtime, breakTime, currentTime.State.IsEntered() || breakTime != null);
        }

        #region Private Methods
        private bool FilterOnlyWorkDays(TimesByDay time, bool onlyWorkDays)
        {
            var workDays = this._applicationStateService.GetWorkDays();
            return onlyWorkDays == false || workDays.Contains(time.Day.DayOfWeek);
        }
        #endregion
    }
}
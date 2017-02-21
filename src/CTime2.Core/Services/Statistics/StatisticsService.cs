using System;
using System.Collections.Generic;
using System.Linq;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IApplicationStateService _applicationStateService;

        public StatisticsService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;
        }

        public TimeSpan CalculateAverageWorkTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var sum = times.Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays)).Sum(f => f.Hours.TotalMinutes);
            var count = times.Count(f => f.Hours != TimeSpan.Zero && this.FilterOnlyWorkDays(f, true));

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

            return TimeSpan.FromMinutes(sum / count);
        }

        public TimeSpan CalculateAverageBreakTime(List<TimesByDay> times, bool onlyWorkDays, bool onlyDaysWithBreak)
        {
            var filteredTimes = times
                .Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays))
                .Where(f => onlyDaysWithBreak == false || f.Times.Count >= 2)
                .ToList();

            var leaveTime = this.CalculateAverageLeaveTime(filteredTimes, onlyWorkDays);
            var enterTime = this.CalculateAverageEnterTime(filteredTimes, onlyWorkDays);
            var workTime = this.CalculateAverageWorkTime(filteredTimes, onlyWorkDays);

            return leaveTime - enterTime - workTime;
        }

        public TimeSpan CalculateOverTime(List<TimesByDay> times, bool onlyWorkDays)
        {
            var dayCount = this.CalculateWorkDayCount(times, onlyWorkDays: true);
            var expectedWorkTime = TimeSpan.FromMinutes(this._applicationStateService.GetWorkDayHours().TotalMinutes * dayCount);
            var actualWorkTime = TimeSpan.FromMinutes(times.Where(f => this.FilterOnlyWorkDays(f, onlyWorkDays)).Sum(f => f.Hours.TotalMinutes));

            return actualWorkTime - expectedWorkTime;
        }

        public TodaysWorkEnd CalculateTodaysWorkEnd(List<TimesByDay> times, bool onlyWorkDays)
        {
            var workDayHours = this._applicationStateService.GetWorkDayHours();
            var workDayBreak = this._applicationStateService.GetWorkDayBreak();

            var timeToday = times.FirstOrDefault(f => f.Day == DateTime.Today);
            var overtime = this.CalculateOverTime(times, onlyWorkDays);

            var latestTimeToday = timeToday?.Times.OrderByDescending(f => f.ClockInTime).FirstOrDefault();

            if (latestTimeToday?.ClockInTime == null)
                return null;

            var workTimeTodayToUseUpOverTimePool = workDayHours
                                                   - overtime
                                                   - (timeToday?.Hours ?? TimeSpan.Zero)
                                                   + (latestTimeToday?.Duration ?? TimeSpan.Zero);
            var hadBreakAlready = timeToday?.Times.Count >= 2;

            var expectedWorkEnd = (latestTimeToday?.ClockInTime ?? DateTime.Now)
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

        #region Private Methods
        private bool FilterOnlyWorkDays(TimesByDay time, bool onlyWorkDays)
        {
            var workDays = this._applicationStateService.GetWorkDays();
            return onlyWorkDays == false || workDays.Contains(time.Day.DayOfWeek);
        }
        #endregion
    }
}
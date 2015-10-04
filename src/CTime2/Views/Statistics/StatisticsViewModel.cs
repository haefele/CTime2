using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Extensions;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Dialog;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;
using CTime2.Strings;
using CTime2.Views.YourTimes;

namespace CTime2.Views.Statistics
{
    public class StatisticsViewModel : Screen
    {
        #region Fields
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        private readonly ILoadingService _loadingService;
        private readonly IDialogService _dialogService;
        private readonly IExceptionHandler _exceptionHandler;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;
        #endregion

        #region Properties
        public DateTimeOffset StartDate
        {
            get { return this._startDate; }
            set { this.SetProperty(ref this._startDate, value); }
        }
        public DateTimeOffset EndDate
        {
            get { return this._endDate; }
            set { this.SetProperty(ref this._endDate, value); }
        }
        public BindableCollection<StatisticItem> Statistics { get; }
        #endregion

        #region Constructors
        public StatisticsViewModel(ISessionStateService sessionStateService, ICTimeService cTimeService, ILoadingService loadingService, IDialogService dialogService, IExceptionHandler exceptionHandler)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
            this._loadingService = loadingService;
            this._dialogService = dialogService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = CTime2Resources.Get("Navigation.Statistics");

            this.Statistics = new BindableCollection<StatisticItem>();

            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }
        #endregion

        #region Methods
        protected override async void OnActivate()
        {
            await this.RefreshAsync();
        }

        public async Task CurrentMonth()
        {
            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();

            await this.RefreshAsync();
        }
        public async Task LastMonth()
        {
            this.StartDate = DateTimeOffset.Now.StartOfMonth().AddMonths(-1);
            this.EndDate = DateTimeOffset.Now.EndOfMonth().AddMonths(-1);

            await this.RefreshAsync();
        }
        public async Task LastSevenDays()
        {
            this.StartDate = DateTimeOffset.Now.WithoutTime().AddDays(-6); //Last 6 days plus today
            this.EndDate = DateTimeOffset.Now.WithoutTime();

            await this.RefreshAsync();
        }
        public async Task RefreshAsync()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.Statistics")))
            {
                try
                {
                    var times = await this._cTimeService.GetTimes(this._sessionStateService.CurrentUser.Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);
                    
                    var timesByDay = TimesByDay.Create(times)
                        .Where(this.IsTimeByDayForStatistic)
                        .ToList();

                    if (times.Count == 0 || timesByDay.Count == 0)
                    {
                        await this._dialogService.ShowAsync(CTime2Resources.Get("Statistics.NoTimesBetweenStartAndEndDate"));
                        return;
                    }

                    var totalWorkTime = TimeSpan.FromMinutes(timesByDay.Where(f => f.Hours != TimeSpan.Zero).Sum(f => f.Hours.TotalMinutes));

                    var totalWorkDays = timesByDay.Count(f => f.Hours != TimeSpan.Zero);

                    var averageWorkTime = TimeSpan.FromMinutes(totalWorkTime.TotalMinutes / totalWorkDays);

                    var averageEnterTime = TimeSpan.FromMinutes(
                        timesByDay.Where(f => f.DayStartTime != null).Sum(f => f.DayStartTime.Value.TotalMinutes) /
                        timesByDay.Count(f => f.DayStartTime != null));

                    var averageLeaveTime = TimeSpan.FromMinutes(
                        timesByDay.Where(f => f.DayEndTime != null).Sum(f => f.DayEndTime.Value.TotalMinutes) /
                        timesByDay.Count(f => f.DayEndTime != null));

                    var averageBreakTime = averageLeaveTime - averageEnterTime - averageWorkTime;

                    this.Statistics.Clear();

                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.AverageWorkTime"), averageWorkTime.ToString("T")));
                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.AverageBreakTime"), averageBreakTime.ToString("T")));
                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.AverageEnterTime"), averageEnterTime.ToDateTime().ToString("T")));
                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.AverageLeaveTime"), averageLeaveTime.ToDateTime().ToString("T")));
                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.TotalWorkDays"), totalWorkDays.ToString()));
                    this.Statistics.Add(new StatisticItem(CTime2Resources.Get("Statistics.TotalWorkTime"), totalWorkTime.ToString(CTime2Resources.Get("Statistics.TotalWorkTimeFormat"))));
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
        private bool IsTimeByDayForStatistic(TimesByDay timesByDay)
        {
            if (timesByDay.Day != DateTime.Today)
                return true;

            if (timesByDay.Times.Count(f => f.ClockOutTime.HasValue) >= 2)
                return true;

            return false;
        }
        #endregion
    }
}
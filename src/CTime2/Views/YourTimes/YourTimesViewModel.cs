using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : Screen
    {
        private readonly IApplicationStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public BindableCollection<TimesByDay> Times { get; }

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

        public YourTimesViewModel(IApplicationStateService sessionStateService, ICTimeService cTimeService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = CTime2Resources.Get("Navigation.MyTimes");

            this.Times = new BindableCollection<TimesByDay>();

            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }

        protected override async void OnActivate()
        {
            await this.RefreshAsync();
        }
        
        public async Task RefreshAsync()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.Times")))
            {
                try
                {
                    var times = await this._cTimeService.GetTimes(this._sessionStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);

                    this.Times.Clear();
                    this.Times.AddRange(TimesByDay.Create(times));
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}
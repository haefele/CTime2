using System;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : ReactiveScreen
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<TimesByDay>> _timesHelper;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public ReactiveObservableCollection<TimesByDay> Times => this._timesHelper.Value;

        public DateTimeOffset StartDate
        {
            get { return this._startDate; }
            set { this.RaiseAndSetIfChanged(ref this._startDate, value); }
        }

        public DateTimeOffset EndDate
        {
            get { return this._endDate; }
            set { this.RaiseAndSetIfChanged(ref this._endDate, value); }
        }

        public ReactiveCommand<ReactiveObservableCollection<TimesByDay>> LoadTimes { get; }

        public YourTimesViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;

            this.DisplayName = CTime2Resources.Get("Navigation.MyTimes");

            this.LoadTimes = ReactiveCommand.CreateAsyncTask(_ => this.LoadTimesImpl());
            this.LoadTimes.AttachExceptionHandler();
            this.LoadTimes.AttachLoadingService(CTime2Resources.Get("Loading.Times"));
            this.LoadTimes.TrackEvent("LoadTimes");
            this.LoadTimes.ToLoadedProperty(this, f => f.Times, out this._timesHelper);

            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }

        protected override async void OnActivate()
        {
            await this.LoadTimes.ExecuteAsyncTask();
        }
        
        private async Task<ReactiveObservableCollection<TimesByDay>> LoadTimesImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);
            return new ReactiveObservableCollection<TimesByDay>(TimesByDay.Create(times));
        }
    }
}
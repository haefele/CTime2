using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.YourTimes
{
    public class YourTimesViewModel : ReactiveScreen
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;

        private readonly ObservableAsPropertyHelper<string> _displayNameHelper;
        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<TimesByDay>> _timesHelper;

        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public override string DisplayName
        {
            get { return this._displayNameHelper.Value; }
            set { }
        }

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

        public UwCoreCommand<ReactiveObservableCollection<TimesByDay>> LoadTimes { get; }

        public YourTimesViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            
            this.WhenAnyValue(f => f.StartDate, f => f.EndDate)
                .Select(f => CTime2Resources.GetFormatted("MyTimes.TitleFormat", this.StartDate, this.EndDate))
                .ToProperty(this, f => f.DisplayName, out this._displayNameHelper);

            this.LoadTimes = UwCoreCommand.Create(this.LoadTimesImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Times"))
                .HandleExceptions()
                .TrackEvent("LoadTimes");
            this.LoadTimes.ToProperty(this, f => f.Times, out this._timesHelper);

            this.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.EndDate = DateTimeOffset.Now.EndOfMonth();
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadTimes.ExecuteAsync();
        }
        
        private async Task<ReactiveObservableCollection<TimesByDay>> LoadTimesImpl()
        {
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.StartDate.LocalDateTime, this.EndDate.LocalDateTime);
            return new ReactiveObservableCollection<TimesByDay>(TimesByDay.Create(times));
        }
    }
}
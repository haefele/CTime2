using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.Sharing;
using CTime2.Strings;
using CTime2.Views.Statistics.Details.BreakTime;
using CTime2.Views.Statistics.Details.EnterAndLeaveTime;
using CTime2.Views.Statistics.Details.OverTime;
using CTime2.Views.Statistics.Details.WorkTime;
using CTime2.Views.YourTimes;
using UwCore;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;
using UwCore.Services.Navigation;

namespace CTime2.Views.Statistics.Details
{
    public class DetailedStatisticViewModel : UwCoreConductor<DetailedStatisticDiagramViewModelBase>
    {
        #region Fields
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;
        private readonly ISharingService _sharingService;
        private readonly IClock _clock;
        #endregion
        
        #region Commands
        public UwCoreCommand<Unit> Load { get; }
        public UwCoreCommand<Unit> GoToMyTimesCommand { get; }
        public UwCoreCommand<Unit> Share { get; }
        #endregion

        #region Parameters
        public Parameters Parameter { get; set; }
        #endregion

        public DetailedStatisticViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, INavigationService navigationService, ISharingService sharingService, IClock clock)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(sharingService, nameof(sharingService));
            Guard.NotNull(clock, nameof(clock));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;
            this._sharingService = sharingService;
            this._clock = clock;

            this.Load = UwCoreCommand.Create(this.LoadImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.LoadCharts"))
                .HandleExceptions();

            this.GoToMyTimesCommand = UwCoreCommand.Create(this.GoToMyTimes)
                .HandleExceptions();

            this.Share = UwCoreCommand.Create(this.ShareImpl)
                .HandleExceptions()
                .TrackEvent("ShareDetailedStatistic");
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.Load.ExecuteAsync();
        }

        private async Task LoadImpl()
        {
            var diagramViewModel = this.GetDiagramViewModel();
            this.ActivateItem(diagramViewModel);

            var workDays = this._applicationStateService.GetWorkDays();
            var times = await this._cTimeService.GetTimes(this._applicationStateService.GetCurrentUser().Id, this.Parameter.StartDate.LocalDateTime, this.Parameter.EndDate.LocalDateTime);

            var timesByDay = TimesByDay.Create(times, workDays)
                .Where(f => f.Day.Date != this._clock.Today() || this.Parameter.IncludeToday)
                .OrderBy(f => f.Day)
                .ToList();

            await diagramViewModel.LoadAsync(timesByDay);
        }

        private Task GoToMyTimes()
        {
            this.TryClose();

            this._navigationService
                .For<YourTimesViewModel>()
                .WithParam(f => f.Parameter.StartDate, this.Parameter.StartDate)
                .WithParam(f => f.Parameter.EndDate, this.Parameter.EndDate)
                .Navigate();

            return Task.CompletedTask;
        }

        private Task ShareImpl()
        {
            this._sharingService.Share(this.DisplayName, async dataPackage =>
            {
                var view = this.GetView() as IDetailedStatisticView;

                if (view == null)
                    throw new InvalidOperationException();

                dataPackage.SetBitmap(await view.GetDiagramAsync());
            });

            return Task.CompletedTask;
        }

        private DetailedStatisticDiagramViewModelBase GetDiagramViewModel()
        {
            switch (this.Parameter.StatisticChart)
            {
                case StatisticChartKind.WorkTime:
                    return IoC.Get<WorkTimeViewModel>();
                case StatisticChartKind.BreakTime:
                    return IoC.Get<BreakTimeViewModel>();
                case StatisticChartKind.EnterAndLeaveTime:
                    return IoC.Get<EnterAndLeaveTimeViewModel>();
                case StatisticChartKind.OverTime:
                    return IoC.Get<OverTimeViewModel>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
                
        #region Internal
        public class Parameters
        {
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public StatisticChartKind StatisticChart { get; set; }
            public bool IncludeToday { get; set; }
        }
        #endregion
    }
}

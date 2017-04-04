using System;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using UwCore;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Clock;

namespace CTime2.Views.Overview
{
    public abstract class StampTimeStateViewModelBase : UwCoreScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IClock _clock;

        public OverviewViewModel Container => this.Parent as OverviewViewModel;
        public abstract TimeState CurrentState { get; }

        public StampTimeStateViewModelBase(ICTimeService cTimeService, IApplicationStateService applicationStateService, IClock clock)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(clock, nameof(clock));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._clock = clock;
        }

        protected async Task Stamp(TimeState state)
        {
            await this._cTimeService.SaveTimer(
                this._applicationStateService.GetCurrentUser(),
                this._clock.Now().DateTime,
                state);

            await this.Container.RefreshCurrentState.ExecuteAsync();
        }
    }
}
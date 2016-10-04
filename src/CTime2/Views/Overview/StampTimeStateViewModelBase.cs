using System;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview
{
    public abstract class StampTimeStateViewModelBase : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        public OverviewViewModel Container => this.Parent as OverviewViewModel;
        public abstract TimeState CurrentState { get; }

        public StampTimeStateViewModelBase(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
        }

        protected async Task Stamp(TimeState state)
        {
            await this._cTimeService.SaveTimer(
                this._applicationStateService.GetCurrentUser(),
                state);

            await this.Container.RefreshCurrentState.ExecuteAsync();
        }
    }
}
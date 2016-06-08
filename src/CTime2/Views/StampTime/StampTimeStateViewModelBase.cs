using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using UwCore.Services.ApplicationState;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.StampTime
{
    public class StampTimeStateViewModelBase : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _sessionStateService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        public StampTimeViewModel Container => this.Parent as StampTimeViewModel;

        public StampTimeStateViewModelBase(ICTimeService cTimeService, IApplicationStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;
        }

        protected async Task Stamp(TimeState state, string resourceKeyForMessage)
        {
            using (this._loadingService.Show(CTime2Resources.Get(resourceKeyForMessage)))
            {
                try
                {
                    await this._cTimeService.SaveTimer(
                        this._sessionStateService.GetCurrentUser().Id,
                        DateTime.Now,
                        this._sessionStateService.GetCurrentUser().CompanyId,
                        state);

                    await this.Container.RefreshCurrentState();
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}
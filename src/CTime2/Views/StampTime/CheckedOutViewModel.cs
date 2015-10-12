using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;
using CTime2.Strings;

namespace CTime2.Views.StampTime
{
    public class CheckedOutViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        public StampTimeViewModel Container => this.Parent as StampTimeViewModel;

        public CheckedOutViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;
        }

        public async void CheckIn()
        {
            await this.CheckInInternal(TimeState.Entered);
        }

        public async void CheckInHomeOffice()
        {
            await this.CheckInInternal(TimeState.Entered | TimeState.HomeOffice);
        }

        public async void CheckInTrip()
        {
            await this.CheckInInternal(TimeState.Entered | TimeState.Trip);
        }

        private async Task CheckInInternal(TimeState state)
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.CheckIn")))
            {
                try
                {
                    await this._cTimeService.SaveTimer(
                        this._sessionStateService.CurrentUser.Id,
                        DateTime.Now,
                        this._sessionStateService.CompanyId,
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
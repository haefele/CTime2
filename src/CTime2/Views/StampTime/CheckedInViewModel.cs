using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class CheckedInViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        public StampTimeViewModel Container => this.Parent as StampTimeViewModel;

        public CheckedInViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;
        }

        public async void CheckOut()
        {
            await this.CheckOutInternal(TimeState.Left, "Loading.CheckOut");
        }

        public async void Pause()
        {
            await this.CheckOutInternal(TimeState.Left | TimeState.ShortBreak, "Loading.Pause");
        }

        private async Task CheckOutInternal(TimeState state, string resourceKeyForMessage)
        {
            using (this._loadingService.Show(CTime2Resources.Get(resourceKeyForMessage)))
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

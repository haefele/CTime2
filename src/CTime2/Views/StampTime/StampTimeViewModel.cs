using System;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Dialog;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Conductor<Screen>
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly IDialogService _dialogService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;
        
        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, IDialogService dialogService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._dialogService = dialogService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Stempeln";
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show("Lade..."))
            {
                try
                {
                    var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                    var isCheckedIn = currentTime != null && currentTime.State.IsEntered();

                    this.ActivateItem(IoC.Get<CheckedInViewModel>());
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}
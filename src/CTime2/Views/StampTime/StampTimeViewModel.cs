using System;
using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Services.CTime;
using CTime2.Services.Dialog;
using CTime2.Services.SessionState;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly IDialogService _dialogService;

        public static bool IsCheckedIn { get; set; }

        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, IDialogService dialogService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._dialogService = dialogService;
        }

        public async void CheckIn()
        {
            if (IsCheckedIn)
            {
                await this._dialogService.ShowAsync("Sie sind bereits eingestempelt!");
                return;
            }

            await this._cTimeService.SaveTimer(
                this._sessionStateService.CurrentUser.Id, 
                DateTime.Now, 
                this._sessionStateService.CompanyId, 
                TimeState.Entered);

            await this._dialogService.ShowAsync($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");

            IsCheckedIn = true;
        }

        public async void CheckOut()
        {
            if (!IsCheckedIn)
            {
                await this._dialogService.ShowAsync("Sie sind bereits ausgestempelt!");
                return;
            }

            await this._cTimeService.SaveTimer(
                this._sessionStateService.CurrentUser.Id,
                DateTime.Now,
                this._sessionStateService.CompanyId,
                TimeState.Left);

            await this._dialogService.ShowAsync($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");

            IsCheckedIn = false;
        }
    }
}
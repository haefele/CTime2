using System;
using Windows.UI.Popups;
using Caliburn.Micro;
using CTime2.Services.CTime;
using CTime2.Services.SessionState;
using CTime2.Views.Overview;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        public static bool IsCheckedIn { get; set; }


        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
        }

        public async void CheckIn()
        {
            if (IsCheckedIn)
            {
                var loggedInMessage = new MessageDialog("Sie sind bereits eingestempelt!");
                await loggedInMessage.ShowAsync();
                return;
            }

            await this._cTimeService.SaveTimer(
                this._sessionStateService.CurrentUser.Id, 
                DateTime.Now, 
                this._sessionStateService.CompanyId, 
                TimeState.Entered);

            var dialog = new MessageDialog($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");
            await dialog.ShowAsync();

            IsCheckedIn = true;
        }

        public async void CheckOut()
        {
            if (!IsCheckedIn)
            {
                var loggedOutMessage = new MessageDialog("Sie sind bereits ausgestempelt!");
                await loggedOutMessage.ShowAsync();
                return;
            }

            await this._cTimeService.SaveTimer(
                this._sessionStateService.CurrentUser.Id,
                DateTime.Now,
                this._sessionStateService.CompanyId,
                TimeState.Left);

            var dialog = new MessageDialog($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");
            await dialog.ShowAsync();

            IsCheckedIn = false;
        }
    }
}
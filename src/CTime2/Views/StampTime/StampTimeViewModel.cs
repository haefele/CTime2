using System;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Loading;
using IDialogService = CTime2.Services.Dialog.IDialogService;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly IDialogService _dialogService;
        private readonly ILoadingService _loadingService;

        private bool _isCheckedIn;

        public bool IsCheckedIn
        {
            get { return this._isCheckedIn; }
            set { this.SetProperty(ref this._isCheckedIn, value); }
        }

        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, IDialogService dialogService, ILoadingService loadingService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._dialogService = dialogService;
            this._loadingService = loadingService;
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show("Lade..."))
            { 
                var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                this.IsCheckedIn = currentTime != null && currentTime.State == TimeState.Entered;
            }
        }

        public async void CheckIn()
        {
            using (this._loadingService.Show("Einstempeln..."))
            { 
                await this._cTimeService.SaveTimer(
                    this._sessionStateService.CurrentUser.Id,
                    DateTime.Now,
                    this._sessionStateService.CompanyId,
                    TimeState.Entered);

                await this._dialogService.ShowAsync($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");

                this.IsCheckedIn = true;
            }
        }

        public async void CheckOut()
        {
            using (this._loadingService.Show("Ausstempeln..."))
            { 
                await this._cTimeService.SaveTimer(
                    this._sessionStateService.CurrentUser.Id,
                    DateTime.Now,
                    this._sessionStateService.CompanyId,
                    TimeState.Left);

                await this._dialogService.ShowAsync($"Hallo {this._sessionStateService.CurrentUser.FirstName}. Deine Zeit wurde gebucht!");

                this.IsCheckedIn = false;
            }
        }
    }
}
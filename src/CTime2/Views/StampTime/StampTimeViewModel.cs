using System;
using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Dialog;
using CTime2.Services.Loading;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly IDialogService _dialogService;
        private readonly ILoadingService _loadingService;
        
        private bool _canCheckIn;
        private bool _canCheckOut;


        public bool CanCheckIn
        {
            get { return this._canCheckIn; }
            set { this.SetProperty(ref this._canCheckIn, value); }
        }

        public bool CanCheckOut
        {
            get { return this._canCheckOut; }
            set { this.SetProperty(ref this._canCheckOut, value); }
        }


        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, IDialogService dialogService, ILoadingService loadingService)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._dialogService = dialogService;
            this._loadingService = loadingService;

            this.DisplayName = "Stempeln";
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show("Lade..."))
            { 
                var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                var isCheckedIn = currentTime != null && currentTime.State.IsEntered();

                this.CanCheckIn = isCheckedIn == false;
                this.CanCheckOut = isCheckedIn;
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

                this.CanCheckIn = false;
                this.CanCheckOut = true;
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

                this.CanCheckIn = true;
                this.CanCheckOut = false;
            }
        }
    }
}
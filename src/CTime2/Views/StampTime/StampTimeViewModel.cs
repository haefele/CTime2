using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Events;
using CTime2.Extensions;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;
using CTime2.Strings;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : Conductor<Screen>, IHandleWithTask<ApplicationResumedEvent>
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        private string _statusMessage;

        public string StatusMessage
        {
            get { return this._statusMessage; }
            set { this.SetProperty(ref this._statusMessage, value); }
        }

        public StampTimeViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = CTime2Resources.Get("Navigation.Stamp");
        }

        protected override async void OnActivate()
        {
            await this.RefreshCurrentState();
        }

        public async Task RefreshCurrentState()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.CurrentState")))
            {
                try
                {
                    var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);

                    string statusMessage;
                    Screen currentState;

                    if (currentTime == null || currentTime.State.IsLeft())
                    {
                        statusMessage = CTime2Resources.Get("StampTime.CurrentlyCheckedOut");
                        currentState = IoC.Get<CheckedOutViewModel>();
                    }
                    else if (currentTime.State.IsEntered() && currentTime.State.IsTrip())
                    {
                        statusMessage = CTime2Resources.Get("StampTime.CurrentlyCheckedInTrip");
                        currentState = IoC.Get<TripCheckedInViewModel>();
                    }
                    else if (currentTime.State.IsEntered() && currentTime.State.IsHomeOffice())
                    {
                        statusMessage = CTime2Resources.Get("StampTime.CurrentlyCheckedInHomeOffice");
                        currentState = IoC.Get<HomeOfficeCheckedInViewModel>();
                    }
                    else if (currentTime.State.IsEntered())
                    {
                        statusMessage = CTime2Resources.Get("StampTime.CurrentlyCheckedIn");
                        currentState = IoC.Get<CheckedInViewModel>();
                    }
                    else
                    {
                        throw new CTimeException("Could not determine the current state.");
                    }

                    this.StatusMessage = statusMessage;
                    this.ActivateItem(currentState);
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }

        public Task Handle(ApplicationResumedEvent message)
        {
            return this.RefreshCurrentState();
        }
    }
}
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using CTime2.Views.StampTime.CheckedIn;
using CTime2.Views.StampTime.CheckedOut;
using CTime2.Views.StampTime.HomeOfficeCheckedIn;
using CTime2.Views.StampTime.TripCheckedIn;
using ReactiveUI;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.StampTime
{
    public class StampTimeViewModel : ReactiveConductor<ReactiveScreen>, IHandleWithTask<ApplicationResumed>
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        
        public ReactiveCommand<Unit> RefreshCurrentState { get; }

        public StampTimeViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            this.DisplayName = CTime2Resources.Get("Navigation.Stamp");

            this.RefreshCurrentState = ReactiveCommand.CreateAsyncTask(_ => this.RefreshCurrentStateImpl());
            this.RefreshCurrentState.AttachExceptionHandler();
            this.RefreshCurrentState.AttachLoadingService(CTime2Resources.Get("Loading.CurrentState"));
        }

        protected override async void OnActivate()
        {
            await this.RefreshCurrentState.ExecuteAsyncTask();
        }

        private async Task RefreshCurrentStateImpl()
        {
            var currentTime = await this._cTimeService.GetCurrentTime(this._applicationStateService.GetCurrentUser().Id);
            
            ReactiveScreen currentState;

            if (currentTime == null || currentTime.State.IsLeft())
            {
                currentState = IoC.Get<CheckedOutViewModel>();
            }
            else if (currentTime.State.IsEntered() && currentTime.State.IsTrip())
            {
                currentState = IoC.Get<TripCheckedInViewModel>();
            }
            else if (currentTime.State.IsEntered() && currentTime.State.IsHomeOffice())
            {
                currentState = IoC.Get<HomeOfficeCheckedInViewModel>();
            }
            else if (currentTime.State.IsEntered())
            {
                currentState = IoC.Get<CheckedInViewModel>();
            }
            else
            {
                throw new CTimeException("Could not determine the current state.");
            }
            
            this.ActivateItem(currentState);
        }

        Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            return this.RefreshCurrentState.ExecuteAsyncTask();
        }
    }
}
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;

namespace CTime2.Views.StampTime
{
    public class CheckedOutViewModel : StampTimeStateViewModelBase
    {
        public CheckedOutViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
            : base(cTimeService, sessionStateService, loadingService, exceptionHandler)
        {
        }

        public async void CheckIn()
        {
            await this.Stamp(TimeState.Entered, "Loading.CheckIn");
        }

        public async void CheckInHomeOffice()
        {
            await this.Stamp(TimeState.Entered | TimeState.HomeOffice, "Loading.CheckIn");
        }

        public async void CheckInTrip()
        {
            await this.Stamp(TimeState.Entered | TimeState.Trip, "Loading.CheckIn");
        }
    }
}
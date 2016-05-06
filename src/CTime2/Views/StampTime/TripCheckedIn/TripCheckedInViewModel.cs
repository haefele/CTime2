using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using UwCore.Services.ApplicationState;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.StampTime.TripCheckedIn
{
    public class TripCheckedInViewModel : StampTimeStateViewModelBase
    {
        public TripCheckedInViewModel(ICTimeService cTimeService, IApplicationStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
            : base(cTimeService, sessionStateService, loadingService, exceptionHandler)
        {
        }
        
        public async void CheckOut()
        {
            await this.Stamp(TimeState.Trip | TimeState.Left, "Loading.CheckOut");
        }

        public async void Pause()
        {
            await this.Stamp(TimeState.ShortBreak | TimeState.Left, "Loading.Pause");
        }
    }
}
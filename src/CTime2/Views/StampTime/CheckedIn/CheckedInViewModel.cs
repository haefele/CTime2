using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using UwCore.Services.ApplicationState;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.StampTime.CheckedIn
{
    public class CheckedInViewModel : StampTimeStateViewModelBase
    {
        public CheckedInViewModel(ICTimeService cTimeService, IApplicationStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
            : base(cTimeService, sessionStateService, loadingService, exceptionHandler)
        {
        }

        public async void CheckOut()
        {
            await this.Stamp(TimeState.Left, "Loading.CheckedOut");
        }

        public async void Pause()
        {
            await this.Stamp(TimeState.Left | TimeState.ShortBreak, "Loading.Pause");
        }
    }
}

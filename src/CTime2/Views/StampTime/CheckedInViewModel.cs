using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;

namespace CTime2.Views.StampTime
{
    public class CheckedInViewModel : StampTimeStateViewModelBase
    {
        public CheckedInViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IExceptionHandler exceptionHandler)
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

using System.Reactive;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview.CheckedIn
{
    public class CheckedInViewModel : StampTimeStateViewModelBase
    {
        public override TimeState CurrentState => TimeState.Entered;

        public UwCoreCommand<Unit> CheckOut { get; }
        public UwCoreCommand<Unit> Pause { get; }

        public CheckedInViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
            : base(cTimeService, applicationStateService)
        {
            this.CheckOut = UwCoreCommand.Create(this.CheckOutImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CheckedOut"))
                .HandleExceptions()
                .TrackEvent("CheckOut");

            this.Pause = UwCoreCommand.Create(this.PauseImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Pause"))
                .HandleExceptions()
                .TrackEvent("Pause");
        }

        private async Task CheckOutImpl()
        {
            await this.Stamp(TimeState.Left);
        }

        private async Task PauseImpl()
        {
            await this.Stamp(TimeState.Left | TimeState.ShortBreak);
        }
    }
}

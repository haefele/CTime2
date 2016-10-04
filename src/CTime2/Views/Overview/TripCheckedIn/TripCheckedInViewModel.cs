using System.Reactive;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview.TripCheckedIn
{
    public class TripCheckedInViewModel : StampTimeStateViewModelBase
    {
        public override TimeState CurrentState => TimeState.Entered | TimeState.Trip;

        public UwCoreCommand<Unit> CheckOut { get; }
        public UwCoreCommand<Unit> Pause { get; }

        public TripCheckedInViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
            : base(cTimeService, applicationStateService)
        {
            this.CheckOut = UwCoreCommand.Create(this.CheckOutImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CheckOut"))
                .HandleExceptions()
                .TrackEvent("CheckOut");

            this.Pause = UwCoreCommand.Create(this.PauseImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.Pause"))
                .HandleExceptions()
                .TrackEvent("Pause");
        }

        private async Task CheckOutImpl()
        {
            await this.Stamp(TimeState.Trip | TimeState.Left);
        }

        private async Task PauseImpl()
        {
            await this.Stamp(TimeState.ShortBreak | TimeState.Left);
        }
    }
}
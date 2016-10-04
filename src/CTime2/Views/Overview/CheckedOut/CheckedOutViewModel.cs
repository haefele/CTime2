using System.Reactive;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview.CheckedOut
{
    public class CheckedOutViewModel : StampTimeStateViewModelBase
    {
        public override TimeState CurrentState => TimeState.Left;

        public UwCoreCommand<Unit> CheckIn { get; }
        public UwCoreCommand<Unit> CheckInHomeOffice { get; }
        public UwCoreCommand<Unit> CheckInTrip { get; }

        public CheckedOutViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
            : base(cTimeService, applicationStateService)
        {
            this.CheckIn = UwCoreCommand.Create(this.CheckInImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CheckIn"))
                .HandleExceptions()
                .TrackEvent("CheckIn");

            this.CheckInHomeOffice = UwCoreCommand.Create(this.CheckInHomeOfficeImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CheckIn"))
                .HandleExceptions()
                .TrackEvent("CheckInHomeOffice");

            this.CheckInTrip = UwCoreCommand.Create(this.CheckInTripImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CheckIn"))
                .HandleExceptions()
                .TrackEvent("CheckInTrip");
        }

        private async Task CheckInImpl()
        {
            await this.Stamp(TimeState.Entered);
        }

        private async Task CheckInHomeOfficeImpl()
        {
            await this.Stamp(TimeState.Entered | TimeState.HomeOffice);
        }

        private async Task CheckInTripImpl()
        {
            await this.Stamp(TimeState.Entered | TimeState.Trip);
        }
    }
}
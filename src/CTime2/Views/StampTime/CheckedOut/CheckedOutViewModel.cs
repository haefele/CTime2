using System.Reactive;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.StampTime.CheckedOut
{
    public class CheckedOutViewModel : StampTimeStateViewModelBase
    {
        public ReactiveCommand<Unit> CheckIn { get; }
        public ReactiveCommand<Unit> CheckInHomeOffice { get; }
        public ReactiveCommand<Unit> CheckInTrip { get; }

        public CheckedOutViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
            : base(cTimeService, applicationStateService)
        {
            this.DisplayName = CTime2Resources.Get("StampTime.CurrentlyCheckedOut");

            this.CheckIn = ReactiveCommand.CreateAsyncTask(_ => this.CheckInImpl());
            this.CheckIn.AttachExceptionHandler();
            this.CheckIn.AttachLoadingService(CTime2Resources.Get("Loading.CheckIn"));

            this.CheckInHomeOffice = ReactiveCommand.CreateAsyncTask(_ => this.CheckInHomeOfficeImpl());
            this.CheckInHomeOffice.AttachExceptionHandler();
            this.CheckInHomeOffice.AttachLoadingService(CTime2Resources.Get("Loading.CheckIn"));

            this.CheckInTrip = ReactiveCommand.CreateAsyncTask(_ => this.CheckInTripImpl());
            this.CheckInTrip.AttachExceptionHandler();
            this.CheckInTrip.AttachLoadingService(CTime2Resources.Get("Loading.CheckIn"));
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
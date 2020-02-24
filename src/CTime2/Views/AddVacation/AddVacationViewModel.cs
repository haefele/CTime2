using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using UwCore;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.AddVacation
{
    public class AddVacationViewModel : UwCoreScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        public UwCoreCommand<Unit> EnterVacation { get; }

        public AddVacationViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            this.DisplayName = "Urlaubsantrag";

            this.EnterVacation = UwCoreCommand.Create(this.EnterVacationImpl)
                .ShowLoadingOverlay("Entering vacation")
                .HandleExceptions()
                .TrackEvent("EnterVacation");
        }

        private async Task EnterVacationImpl()
        {
            await this._cTimeService.SaveHolidayRequest(
                this._applicationStateService.GetCurrentUser().Id,
                new DateTime(2020, 4, 1), 
                new DateTime(2020, 4, 9), 
                HolidayKind.WholeDay, 
                false, 
                "c-Time Universal");
        }
    }
}

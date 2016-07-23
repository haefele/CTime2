﻿using System.Reactive;
using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Overview.TripCheckedIn
{
    public class TripCheckedInViewModel : StampTimeStateViewModelBase
    {
        public override TimeState CurrentState => TimeState.Entered | TimeState.Trip;

        public ReactiveCommand<Unit> CheckOut { get; }
        public ReactiveCommand<Unit> Pause { get; }

        public TripCheckedInViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
            : base(cTimeService, applicationStateService)
        {
            this.CheckOut = ReactiveCommand.CreateAsyncTask(_ => this.CheckOutImpl());
            this.CheckOut.AttachExceptionHandler();
            this.CheckOut.AttachLoadingService(CTime2Resources.Get("Loading.CheckOut"));
            this.CheckOut.TrackEvent("CheckOut");

            this.Pause = ReactiveCommand.CreateAsyncTask(_ => this.PauseImpl());
            this.Pause.AttachExceptionHandler();
            this.Pause.AttachLoadingService(CTime2Resources.Get("Loading.Pause"));
            this.Pause.TrackEvent("Pause");
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
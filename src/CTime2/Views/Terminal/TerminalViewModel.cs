﻿using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Terminal
{
    public class TerminalViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private string _rfidKey;

        public string RfidKey
        {
            get { return this._rfidKey; }
            set { this.RaiseAndSetIfChanged(ref this._rfidKey, value); }
        }

        public UwCoreCommand<Unit> Stamp { get; }

        public TerminalViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            var canStamp = this.WhenAnyValue(f => f.RfidKey, (string rfidKey) => string.IsNullOrWhiteSpace(rfidKey) == false);
            this.Stamp = UwCoreCommand.Create(canStamp, this.StampImpl)
                .HandleExceptions()
                .ShowLoadingOverlay("Stempeln...");
        }

        private async Task StampImpl()
        {
            Debug.WriteLine(this.RfidKey);
            
            await this._cTimeService.SaveTimer(
                string.Empty, 
                this.RfidKey, 
                DateTime.Now, 
                this._applicationStateService.GetCompanyId(), 
                TimeState.Entered,
                true);

            this.RfidKey = string.Empty;
        }
    }
}
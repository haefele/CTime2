using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Logging;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Terminal
{
    public enum TerminalStampMode
    {
        Normal,
        HomeOffice,
        Trip,
        Pause,
    }

    public class TerminalViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private readonly Timer _timer;
        private readonly Timer _resetModeTimer;

        private TimeSpan _currentTime;
        private string _rfidKey;
        private TerminalStampMode _stampMode;

        public TimeSpan CurrentTime
        {
            get { return this._currentTime; }
            set { this.RaiseAndSetIfChanged(ref this._currentTime, value); }
        }

        public string RfidKey
        {
            get { return this._rfidKey; }
            set { this.RaiseAndSetIfChanged(ref this._rfidKey, value); }
        }

        public TerminalStampMode StampMode
        {
            get { return this._stampMode; }
            set { this.RaiseAndSetIfChanged(ref this._stampMode, value); }
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

            this.StampMode = TerminalStampMode.Normal;
            this.WhenAnyValue(f => f.StampMode)
                .Subscribe(stampMode =>
                {
                    if (stampMode == TerminalStampMode.Normal)
                    {
                        //Stop the timer
                        this._resetModeTimer?.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(10));
                    }
                    else
                    {
                        //Start the timer
                        this._resetModeTimer?.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
                    }
                });

            this._timer = new Timer(this.Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            this._resetModeTimer = new Timer(this.ResetModeTick, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(10));

            this.DisplayName = "Terminal";
        }

        private void Tick(object state)
        {
            Execute.OnUIThread(() =>
            {
                this.CurrentTime = DateTimeOffset.Now.TimeOfDay;
            });
        }

        private void ResetModeTick(object state)
        {
            Execute.OnUIThread(() =>
            {
                if (this.StampMode != TerminalStampMode.Normal)
                {
                    this.StampMode = TerminalStampMode.Normal;
                }

                //Stop the timer
                this._resetModeTimer?.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(10));
            });
        }

        private async Task StampImpl()
        {
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
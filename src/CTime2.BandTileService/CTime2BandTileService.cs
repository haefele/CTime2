using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using CTime2.Core.Data;
using CTime2.Core.Logging;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using Microsoft.Band;

namespace CTime2.BandTileService
{
    public sealed class CTime2BandTileService : IBackgroundTask, IDisposable
    {
        #region Logger
        private static readonly Logger _logger = LoggerFactory.GetLogger<CTime2BandTileService>();
        #endregion

        private BackgroundTaskDeferral _deferral;
        private IDisposable _disposable;
        private IBandService _bandService;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();

            try
            {
                taskInstance.Canceled += (s, e) => this.Dispose();

                this._bandService = new BandService();

                this._disposable = await this._bandService.ListenForEventsAsync(
                    async f => await this.Stamp(f, TimeState.Entered),
                    async f => await this.Stamp(f, TimeState.Left));
            }
            catch (Exception exception)
            {
                _logger.Error(exception, () => "Exception occured in the band tile service.");
                this.Dispose();
            }
        }
        
        private async Task Stamp(IBandClient client, TimeState timeState)
        {
            var sessionStateService = new SessionStateService();
            await sessionStateService.RestoreStateAsync();

            if (sessionStateService.CurrentUser == null)
            {
                _logger.Debug(() => "User is not logged in.");
                await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Nicht eingeloggt.");

                return;
            }

            var cTimeService = new CTimeService();

            var currentTime = await cTimeService.GetCurrentTime(sessionStateService.CurrentUser.Id);
            bool checkedIn = currentTime != null && currentTime.State.IsEntered();

            if (checkedIn && timeState.IsEntered())
            {
                _logger.Debug(() => "User wants to check-in. But he is already. Asking him if he wants to check-out instead.");

                await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits eingestempelt.");
                return;
            }

            if (checkedIn == false && timeState.IsLeft())
            {
                _logger.Debug(() => "User wants to check-out. But he is already. Asking him if he wants to check-in instead.");

                await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits ausgestempelt.");
                return;
            }

            if (timeState.IsLeft())
            {
                _logger.Debug(() => "User is checking out. Updating the TimeState to make him check out what he previously checked in (Normal, Trip or Home-Office).");
                if (currentTime.State.IsTrip())
                {
                    _logger.Debug(() => "User checked-in a trip. Update the TimeState to make him check out a trip.");
                    timeState = timeState | TimeState.Trip;
                }

                if (currentTime.State.IsHomeOffice())
                {
                    _logger.Debug(() => "User checked-in home-office. Update the TimeState to make him check out home-office.");
                    timeState = timeState | TimeState.HomeOffice;
                }
            }

            _logger.Debug(() => "Saving the timer.");
            await cTimeService.SaveTimer(sessionStateService.CurrentUser.Id, DateTime.Now, sessionStateService.CompanyId, timeState);

            _logger.Debug(() => "Finished voice command.");
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Erfolgreich gestempelt.");
        }

        public void Dispose()
        {
            this._disposable.Dispose();
            this._deferral.Complete();
        }
    }
}

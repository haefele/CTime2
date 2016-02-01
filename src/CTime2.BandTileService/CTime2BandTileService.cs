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
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Oi");
        }

        public void Dispose()
        {
            this._disposable.Dispose();
            this._deferral.Complete();
        }
    }
}

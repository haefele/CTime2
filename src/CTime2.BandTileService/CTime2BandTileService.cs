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

namespace CTime2.BandTileService
{
    public sealed class CTime2BandTileService : IBackgroundTask, IDisposable
    {
        #region Logger
        private static readonly Logger _logger = LoggerFactory.GetLogger<CTime2BandTileService>();
        #endregion

        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();

            try
            {
                taskInstance.Canceled += (s, e) => this.Dispose();

                var bandService = new BandService();
                
                bandService.CheckInPressed += async (s, e) =>
                {
                    await this.Stamp(bandService, TimeState.Entered);
                };
                bandService.CheckOutPressed += async (s, e) =>
                {
                    await this.Stamp(bandService, TimeState.Left);
                };

                await bandService.StartListeningForEvents();
            }
            catch (Exception exception)
            {
                _logger.Error(exception, () => "Exception occured in the band tile service.");    
            }
        }

        private async Task Stamp(BandService bandService, TimeState timeState)
        {
        }

        public void Dispose()
        {
            this._deferral.Complete();
        }
    }
}

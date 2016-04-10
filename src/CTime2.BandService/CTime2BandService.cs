using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using CTime2.Core.Logging;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using Microsoft.Band;

namespace CTime2.BandService
{
    public sealed class CTime2BandService : IBackgroundTask
    {
        #region Logger
        private static readonly Logger Logger = LoggerFactory.GetLogger<CTime2BandService>();
        #endregion

        #region Implementation of IBackgroundTask
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;

                if (triggerDetails.Name == "com.microsoft.band.observer")
                {
                    var sessionStateService = new SessionStateService();
                    var cTimeService = new CTimeService();

                    var bandService = new Core.Services.Band.BandService(sessionStateService, cTimeService);

                    triggerDetails.AppServiceConnection.RequestReceived += async (sender, e) =>
                    {
                        var deferral = e.GetDeferral();
                        await sessionStateService.RestoreStateAsync(); //Restore session state with every event
                        await e.Request.SendResponseAsync(new ValueSet());
                        deferral.Complete();

                        //Handle the tile event outside of the deferral
                        //Otherwise the band will just queue up the tile events
                        //And we want to not allow the user to press buttons multiple times
                        //So we let our own method handle waiting or not
                        await bandService.HandleTileEventAsync(e.Request.Message);
                    };
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "An error occurred.");
            }
        }
        #endregion
    }
}
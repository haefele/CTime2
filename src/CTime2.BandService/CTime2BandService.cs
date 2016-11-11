using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Caliburn.Micro;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.GeoLocation;
using Microsoft.Band;
using UwCore.Logging;
using UwCore.Services.ApplicationState;

namespace CTime2.BandService
{
    public sealed class CTime2BandService : IBackgroundTask
    {
        #region Logger
        private static readonly ILog Logger = LogManager.GetLog(typeof(CTime2BandService));

        private BackgroundTaskDeferral _deferral;
        #endregion

        #region Implementation of IBackgroundTask
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                this._deferral = taskInstance.GetDeferral();

                taskInstance.Canceled += (s, e) => this.Close();

                var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;

                if (triggerDetails.Name == "com.microsoft.band.observer")
                {
                    var applicationStateService = new ApplicationStateService();
                    var cTimeService = new CTimeService(new EventAggregator(), applicationStateService, new GeoLocationService());

                    var bandService = new Core.Services.Band.BandService(applicationStateService, cTimeService);

                    triggerDetails.AppServiceConnection.RequestReceived += async (sender, e) =>
                    {
                        var deferral = e.GetDeferral();
                        await applicationStateService.RestoreStateAsync(); //Restore session state with every event
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
                Logger.Error(exception);

                this.Close();
            }
        }

        private void Close()
        {
            this._deferral.Complete();
        }

        #endregion
    }
}
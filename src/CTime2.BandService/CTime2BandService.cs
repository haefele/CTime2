using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using Microsoft.Band;

namespace CTime2.BandService
{
    public sealed class CTime2BandService : IBackgroundTask
    {
        #region Fields
        private BackgroundTaskDeferral _deferral;
        #endregion

        #region Implementation of IBackgroundTask
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();
            
            try
            {
                taskInstance.Canceled += (s, e) => this.Close();

                var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;

                if (triggerDetails.Name == "com.microsoft.band.observer")
                {
                    var sessionStateService = new SessionStateService();
                    await sessionStateService.RestoreStateAsync();
                    var cTimeService = new CTimeService();

                    var bandService = new Core.Services.Band.BandService(sessionStateService, cTimeService);

                    triggerDetails.AppServiceConnection.RequestReceived += async (sender, e) =>
                    {
                        var deferral = e.GetDeferral();

                        bandService.HandleTileEvent(e.Request.Message);
                        await e.Request.SendResponseAsync(new ValueSet());

                        deferral.Complete();
                    };
                }
            }
            catch
            {
                this.Close();
            }
        }
        #endregion
        
        #region Private Methods
        private void Close()
        {
            this._deferral.Complete();
        }
        #endregion
    }
}
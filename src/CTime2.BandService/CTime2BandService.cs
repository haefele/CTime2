using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using CTime2.Core.Services.Band;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;

namespace CTime2.BandService
{
    public sealed class CTime2BandService : IBackgroundTask
    {
        #region Fields
        private BackgroundTaskDeferral _deferral;
        #endregion

        #region Implementation of IBackgroundTask
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();
            
            try
            {
                taskInstance.Canceled += (s, e) => this.Close();

                var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;

                if (triggerDetails.Name == "com.microsoft.band.observer")
                {
                    var bandService = new Core.Services.Band.BandService(new SessionStateService(), new CTimeService());

                    triggerDetails.AppServiceConnection.RequestReceived += async (sender, e) =>
                    {
                        await bandService.HandleTileEventAsync(e.Request.Message);
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
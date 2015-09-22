using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace CTime2.VoiceCommandService
{
    public sealed class CTime2VoiceCommandService : IBackgroundTask, IDisposable
    {
        private BackgroundTaskDeferral _deferral;
        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += (s, e) => this.Dispose();

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null &&
                triggerDetails.Name == "CTime2VoiceCommandService")
            {
                var voiceCommandServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                voiceCommandServiceConnection.VoiceCommandCompleted += (s, e) => this.Dispose();

                var voiceCommand = await voiceCommandServiceConnection.GetVoiceCommandAsync();

                if (voiceCommand.CommandName == "checkIn")
                {
                    //TODO: Check In
                }
                else if (voiceCommand.CommandName == "checkOut")
                {
                    //TODO: Check out
                }
            }
        }
        
        public void Dispose()
        {
            this._deferral?.Complete();
        }
    }
}

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;

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

                switch (voiceCommand.CommandName)
                {
                    case "checkIn":
                        await this.SaveTimer(voiceCommandServiceConnection, TimeState.Entered);
                        break;
                    case "checkOut":
                        await this.SaveTimer(voiceCommandServiceConnection, TimeState.Left);
                        break;
                }
            }
        }

        private async Task SaveTimer(VoiceCommandServiceConnection connection, TimeState timeState)
        {
            var sessionStateService = new SessionStateService();
            await sessionStateService.RestoreStateAsync();

            if (sessionStateService.CurrentUser == null)
            {
                await connection.ReportFailureAsync(VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage
                {
                    SpokenMessage = "Leider bist du nicht angemeldet.",
                }));

                return;
            }

            var cTimeService = new CTimeService();

            var currentTime = await cTimeService.GetCurrentTime(sessionStateService.CurrentUser.Id);
            bool checkedIn = currentTime?.State == TimeState.Entered;

            if (checkedIn && timeState == TimeState.Entered)
            {
                await connection.ReportFailureAsync(VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage
                {
                    SpokenMessage = "Leider bist du bereits eingestempelt."
                }));

                return;
            }

            if (checkedIn == false && timeState == TimeState.Left)
            {
                await connection.ReportFailureAsync(VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage
                {
                    SpokenMessage = "Leider bist du bereits ausgestempelt."
                }));

                return;
            }

            await cTimeService.SaveTimer(sessionStateService.CurrentUser.Id, DateTime.Now, sessionStateService.CompanyId, timeState);
        }

        public void Dispose()
        {
            this._deferral?.Complete();
        }
    }
}

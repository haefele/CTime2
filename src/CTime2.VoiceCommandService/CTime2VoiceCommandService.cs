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

            try
            {
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
            finally
            {
                this.Dispose();
            }
        }

        private async Task SaveTimer(VoiceCommandServiceConnection connection, TimeState timeState)
        {
            var sessionStateService = new SessionStateService();
            await sessionStateService.RestoreStateAsync();

            if (sessionStateService.CurrentUser == null)
            {
                await connection.ReportFailureAsync(this.NotLoggedInResponse());

                return;
            }

            var cTimeService = new CTimeService();

            var currentTime = await cTimeService.GetCurrentTime(sessionStateService.CurrentUser.Id);
            bool checkedIn = currentTime?.State == TimeState.Entered;

            if (checkedIn && timeState == TimeState.Entered)
            {
                var checkOutResult = await connection.RequestConfirmationAsync(this.AskIfCheckOutResponse());

                if (checkOutResult?.Confirmed == false)
                {
                    await connection.ReportFailureAsync(this.DidNothingResponse());
                    return;
                }

                timeState = TimeState.Left;
            }

            if (checkedIn == false && timeState == TimeState.Left)
            {
                var checkInResult = await connection.RequestConfirmationAsync(this.AskIfCheckInResponse());

                if (checkInResult?.Confirmed == false)
                {
                    await connection.ReportFailureAsync(this.DidNothingResponse());
                    return;
                }

                timeState = TimeState.Entered;
            }

            await cTimeService.SaveTimer(sessionStateService.CurrentUser.Id, DateTime.Now, sessionStateService.CompanyId, timeState);

            await connection.ReportSuccessAsync(this.FinishedResponse(timeState));
        }

        public void Dispose()
        {
            this._deferral?.Complete();
        }

        private VoiceCommandResponse FinishedResponse(TimeState timeState)
        {
            var message = new VoiceCommandUserMessage
            {
                DisplayMessage = string.Format("Erfolgreich {0}.", (timeState == TimeState.Entered ? "eingestempelt" : "ausgestempelt")),
                SpokenMessage = string.Format("Erfolgreich {0}.", (timeState == TimeState.Entered ? "eingestempelt" : "ausgestempelt"))
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse DidNothingResponse()
        {
            var message = new VoiceCommandUserMessage
            {
                DisplayMessage = "Nicht gestempelt.",
                SpokenMessage = "Nicht gestempelt."
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse NotLoggedInResponse()
        {
            var message = new VoiceCommandUserMessage
            {
                DisplayMessage = "Nicht angemeldet.",
                SpokenMessage = "Leider bist du nicht angemeldet.",
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse AskIfCheckOutResponse()
        {
            var promptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = "Bereits eingestempelt. Ausstempeln?",
                SpokenMessage = "Du bist bereits eingestempelt. Möchtest du dich ausstempeln?"
            };
            var rePromptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = "Bereits eingestempelt. Möchtest du dich ausstempeln?",
                SpokenMessage = "Du bist bereits eingestempelt. Möchtest du dich stattdessen ausstempeln?",
            };

            return VoiceCommandResponse.CreateResponseForPrompt(promptMessage, rePromptMessage);
        }

        private VoiceCommandResponse AskIfCheckInResponse()
        {
            var promptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = "Bereits ausgestempelt. Einstempeln?",
                SpokenMessage = "Du bist bereits ausgestempelt. Möchtest du dich einstempeln?"
            };
            var rePromptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = "Bereits ausgestempelt. Möchtest du dich einstempeln?",
                SpokenMessage = "Du bist bereits ausgestempelt. Möchtest du dich stattdessen einstempeln?",
            };

            return VoiceCommandResponse.CreateResponseForPrompt(promptMessage, rePromptMessage);
        }
    }
}

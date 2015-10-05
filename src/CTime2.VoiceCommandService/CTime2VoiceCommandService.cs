using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using CTime2.Core.Data;
using CTime2.Core.Logging;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.VoiceCommandService.Strings;

namespace CTime2.VoiceCommandService
{
    public sealed class CTime2VoiceCommandService : IBackgroundTask, IDisposable
    {
        #region Logger
        private static readonly Logger _logger = LoggerFactory.GetLogger<CTime2VoiceCommandService>();
        #endregion

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

                    _logger.Debug(() => $"Executing voice command '{voiceCommand.CommandName}'.");

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
            catch (Exception exception)
            {
                _logger.Error(exception, () => "Exception occured in the voice command service.");
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
                _logger.Debug(() => "User is not logged in.");
                await connection.ReportFailureAsync(this.NotLoggedInResponse());

                return;
            }

            var cTimeService = new CTimeService();

            var currentTime = await cTimeService.GetCurrentTime(sessionStateService.CurrentUser.Id);
            bool checkedIn = currentTime != null && currentTime.State.IsEntered();

            if (checkedIn && timeState.IsEntered())
            {
                _logger.Debug(() => "User wants to check-in. But he is already. Asking him if he wants to check-out instead.");
                var checkOutResult = await connection.RequestConfirmationAsync(this.AskIfCheckOutResponse());

                if (checkOutResult?.Confirmed == false)
                {
                    _logger.Debug(() => "User does not want to check-out. Doing nothing.");
                    await connection.ReportFailureAsync(this.DidNothingResponse());
                    return;
                }

                timeState = TimeState.Left;
            }

            if (checkedIn == false && timeState.IsLeft())
            {
                _logger.Debug(() => "User wants to check-out. But he is already. Asking him if he wants to check-in instead.");
                var checkInResult = await connection.RequestConfirmationAsync(this.AskIfCheckInResponse());

                if (checkInResult?.Confirmed == false)
                {
                    _logger.Debug(() => "User does not want to check-in. Doing nothing.");
                    await connection.ReportFailureAsync(this.DidNothingResponse());
                    return;
                }

                timeState = TimeState.Entered;
            }

            _logger.Debug(() => "Saving the timer.");
            await cTimeService.SaveTimer(sessionStateService.CurrentUser.Id, DateTime.Now, sessionStateService.CompanyId, timeState);

            _logger.Debug(() => "Finished voice command.");
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
                DisplayMessage = timeState.IsEntered() 
                    ? CTime2VoiceCommandServiceResources.Get("SuccessfullyCheckedIn")
                    : CTime2VoiceCommandServiceResources.Get("SuccessfullyCheckedOut"),
                SpokenMessage = timeState.IsEntered()
                    ? CTime2VoiceCommandServiceResources.Get("SuccessfullyCheckedIn")
                    : CTime2VoiceCommandServiceResources.Get("SuccessfullyCheckedOut"),
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse DidNothingResponse()
        {
            var message = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("DidNothing"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("DidNothing"),
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse NotLoggedInResponse()
        {
            var message = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("NotLoggedInDisplayMessage"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("NotLoggedInSpokenMessage"),
            };

            return VoiceCommandResponse.CreateResponse(message);
        }

        private VoiceCommandResponse AskIfCheckOutResponse()
        {
            var promptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedInDisplayMessage"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedInSpokenMessage"),
            };
            var rePromptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedInDisplayMessageRepeat"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedInSpokenMessageRepeat"),
            };

            return VoiceCommandResponse.CreateResponseForPrompt(promptMessage, rePromptMessage);
        }

        private VoiceCommandResponse AskIfCheckInResponse()
        {
            var promptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedOutDisplayMessage"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedOutSpokenMessage"),
            };
            var rePromptMessage = new VoiceCommandUserMessage
            {
                DisplayMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedOutDisplayMessageRepeat"),
                SpokenMessage = CTime2VoiceCommandServiceResources.Get("AlreadyCheckedOutSpokenMessageRepeat"),
            };
            return VoiceCommandResponse.CreateResponseForPrompt(promptMessage, rePromptMessage);
        }
    }
}

using System;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Logging;

namespace CTime2.Views.About
{
    public class AboutViewModel : UwCoreScreen
    {
        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.RaiseAndSetIfChanged(ref this._currentVersion, value); }
        }

        public UwCoreCommand<Unit> SendFeedback { get; }

        public AboutViewModel()
        {
            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();

            this.SendFeedback = UwCoreCommand.Create(this.SendFeedbackImpl)
                .HandleExceptions()
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.SendFeedback"))
                .TrackEvent("SendFeedback");
        }

        private async Task SendFeedbackImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("Feedback.EmailAddress")));
            message.Subject = CTime2Resources.Get("Feedback.Subject");
            
            await EmailManager.ShowComposeNewEmailAsync(message);
        }
    }
}
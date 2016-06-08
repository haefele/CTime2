using System;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Caliburn.Micro.ReactiveUI;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Logging;
using UwCore.Extensions;

namespace CTime2.Views.Settings.About
{
    public class AboutViewModel : ReactiveScreen
    {
        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.RaiseAndSetIfChanged(ref this._currentVersion, value); }
        }

        public ReactiveCommand<Unit> SendFeedback { get; }

        public AboutViewModel()
        {
            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();

            this.SendFeedback = ReactiveCommand.CreateAsyncTask(_ => this.SendFeedbackImpl());
            this.SendFeedback.AttachExceptionHandler();
        }

        private async Task SendFeedbackImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("Feedback.EmailAddress")));
            message.Subject = CTime2Resources.Get("Feedback.Subject");

            var logs = await LoggerFactory.GetCompressedLogs();
            if (logs != null)
            {
                var reference = RandomAccessStreamReference.CreateFromStream(await logs.ToRandomAccessStreamAsync());
                message.Attachments.Add(new EmailAttachment(CTime2Resources.Get("Feedback.LogsFileName"), reference));
            }

            await EmailManager.ShowComposeNewEmailAsync(message);
        }
    }
}
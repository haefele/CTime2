using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Caliburn.Micro;
using CTime2.Strings;
using UwCore.Logging;
using UwCore.Extensions;

namespace CTime2.Views.Settings.About
{
    public class AboutViewModel : Screen
    {
        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.SetProperty(ref this._currentVersion, value); }
        }

        public AboutViewModel()
        {
            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();
        }

        public async void SendFeedbackAsync()
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
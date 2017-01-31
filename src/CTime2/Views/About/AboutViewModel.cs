using System;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Markup;
using CTime2.Strings;
using CTime2.Views.UpdateNotes;
using Microsoft.Services.Store.Engagement;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Dialog;
using UwCore.Services.Navigation;

namespace CTime2.Views.About
{
    public class AboutViewModel : UwCoreScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.RaiseAndSetIfChanged(ref this._currentVersion, value); }
        }

        public UwCoreCommand<Unit> OpenCTimeWebsite { get; }
        public UwCoreCommand<Unit> SendCTimeMail { get; }
        public UwCoreCommand<Unit> OpenGitHubProject { get; }
        public UwCoreCommand<Unit> ShowPatchNotes { get; }
        public UwCoreCommand<Unit> SendFeedbackPerMail { get; }
        public UwCoreCommand<Unit> SendFeedbackPerFeedbackHub { get; }
        public UwCoreCommand<Unit> ShowPrivacyPolicy { get; }
        public UwCoreCommand<Unit> SendMail { get; }
        public UwCoreCommand<Unit> OpenTwitter { get; }
        public UwCoreCommand<Unit> OpenGitHub { get; }

        public AboutViewModel(INavigationService navigationService, IDialogService dialogService)
        {
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(dialogService, nameof(dialogService));

            this._navigationService = navigationService;
            this._dialogService = dialogService;

            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();

            this.OpenCTimeWebsite = UwCoreCommand.Create(this.OpenCTimeWebsiteImpl)
                .HandleExceptions()
                .TrackEvent("OpenCTimeWebsite");

            this.SendCTimeMail = UwCoreCommand.Create(this.SendCTimeMailImpl)
                .HandleExceptions()
                .TrackEvent("SendCTimeMail");

            this.OpenGitHubProject = UwCoreCommand.Create(this.OpenGitHubProjectImpl)
                .HandleExceptions()
                .TrackEvent("OpenGitHubProject");

            this.ShowPatchNotes = UwCoreCommand.Create(this.ShowPatchNotesImpl)
                .HandleExceptions()
                .TrackEvent("ShowPatchNotes");

            this.SendFeedbackPerMail = UwCoreCommand.Create(this.SendFeedbackPerMailImpl)
                .HandleExceptions()
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.SendFeedback"))
                .TrackEvent("SendFeedbackPerMail");

            this.SendFeedbackPerFeedbackHub = UwCoreCommand.Create(this.SendFeedbackPerFeedbackHubImpl)
                .HandleExceptions()
                .TrackEvent("SendFeedbackPerFeedbackHub");

            this.ShowPrivacyPolicy = UwCoreCommand.Create(this.ShowPrivacyPolicyImpl)
                .HandleExceptions()
                .TrackEvent("ShowPrivacyPolicy");

            this.SendMail = UwCoreCommand.Create(this.SendMailImpl)
                .HandleExceptions()
                .TrackEvent("SendMail");

            this.OpenTwitter = UwCoreCommand.Create(this.OpenTwitterImpl)
                .HandleExceptions()
                .TrackEvent("OpenTwitter");

            this.OpenGitHub = UwCoreCommand.Create(this.OpenGitHubImpl)
                .HandleExceptions()
                .TrackEvent("OpenGitHub");
        }

        private async Task OpenCTimeWebsiteImpl()
        {
            await Launcher.LaunchUriAsync(new Uri(CTime2Resources.Get("CTimeWebsite")));
        }

        private async Task SendCTimeMailImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("CTimeMailAddress")));

            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        private async Task OpenGitHubProjectImpl()
        {
            await Launcher.LaunchUriAsync(new Uri(CTime2Resources.Get("CTimeUniversalGitHubUrl")));
        }

        private Task ShowPatchNotesImpl()
        {
            this._navigationService.Popup.For<UpdateNotesViewModel>().Navigate();

            return Task.CompletedTask;
        }

        private async Task SendFeedbackPerMailImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("Feedback.EmailAddress")));
            message.Subject = CTime2Resources.Get("Feedback.Subject");

            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        private async Task ShowPrivacyPolicyImpl()
        {
            await Launcher.LaunchUriAsync(new Uri(CTime2Resources.Get("PrivacyPolicyUrl")));
        }

        private async Task SendFeedbackPerFeedbackHubImpl()
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        private async Task SendMailImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("Feedback.EmailAddress")));

            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        private async Task OpenTwitterImpl()
        {
            await Launcher.LaunchUriAsync(new Uri(CTime2Resources.Get("TwitterUrl")));
        }

        private async Task OpenGitHubImpl()
        {
            await Launcher.LaunchUriAsync(new Uri(CTime2Resources.Get("GitHubUrl")));
        }
    }
}
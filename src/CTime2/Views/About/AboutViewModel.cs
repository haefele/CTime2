using System;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using CTime2.Strings;
using CTime2.Views.UpdateNotes;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Navigation;

namespace CTime2.Views.About
{
    public class AboutViewModel : UwCoreScreen
    {
        private readonly INavigationService _navigationService;

        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.RaiseAndSetIfChanged(ref this._currentVersion, value); }
        }

        public UwCoreCommand<Unit> SendFeedback { get; }
        public UwCoreCommand<Unit> PatchNotes { get; }

        public AboutViewModel(INavigationService navigationService)
        {
            Guard.NotNull(navigationService, nameof(navigationService));

            this._navigationService = navigationService;

            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();

            this.SendFeedback = UwCoreCommand.Create(this.SendFeedbackImpl)
                .HandleExceptions()
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.SendFeedback"))
                .TrackEvent("SendFeedback");

            this.PatchNotes = UwCoreCommand.Create(this.PatchNotesImpl)
                .HandleExceptions()
                .TrackEvent("PatchNotes");
        }

        private async Task SendFeedbackImpl()
        {
            var message = new EmailMessage();
            message.To.Add(new EmailRecipient(CTime2Resources.Get("Feedback.EmailAddress")));
            message.Subject = CTime2Resources.Get("Feedback.Subject");
            
            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        private Task PatchNotesImpl()
        {
            this._navigationService.Popup.For<UpdateNotesViewModel>().Navigate();

            return Task.CompletedTask;
        }
    }
}
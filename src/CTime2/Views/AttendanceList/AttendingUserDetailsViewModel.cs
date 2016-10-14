using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;
using Windows.Foundation.Metadata;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Contacts;
using CTime2.Core.Services.CTime;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;

namespace CTime2.Views.AttendanceList
{
    public class AttendingUserDetailsViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IContactsService _contactsService;
        private readonly IDialogService _dialogService;

        private readonly ObservableAsPropertyHelper<AttendingUser> _attendingUserHelper;
        
        public AttendingUser AttendingUser => this._attendingUserHelper.Value;

        public string AttendingUserId { get; set; }

        public UwCoreCommand<AttendingUser> LoadAttendingUser { get; }
        public UwCoreCommand<Unit> Call { get; }
        public UwCoreCommand<Unit> SendMail { get; }
        public UwCoreCommand<Unit> AddAsContact { get; }

        public AttendingUserDetailsViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, IContactsService contactsService, IDialogService dialogService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(contactsService, nameof(contactsService));
            Guard.NotNull(dialogService, nameof(dialogService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._contactsService = contactsService;
            this._dialogService = dialogService;

            this.LoadAttendingUser = UwCoreCommand.Create(this.LoadAttendingUserImpl)
                .HandleExceptions()
                .ShowLoadingOverlay("Lade Mitarbeiter");
            this.LoadAttendingUser.ToProperty(this, f => f.AttendingUser, out this._attendingUserHelper);
            
            var canCall = Observable
                .Return(ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1))
                .CombineLatest(this.WhenAnyValue(f => f.AttendingUser), (apiAvailable, user) => 
                    apiAvailable && string.IsNullOrWhiteSpace(user?.PhoneNumber) == false);
            this.Call = UwCoreCommand.Create(canCall, this.CallImpl)
                .HandleExceptions();

            var canSendMail = this.WhenAnyValue(f => f.AttendingUser, selector: user => string.IsNullOrWhiteSpace(user?.EmailAddress) == false);
            this.SendMail = UwCoreCommand.Create(canSendMail, this.SendMailImpl)
                .HandleExceptions();

            this.AddAsContact = UwCoreCommand.Create(this.AddAsContactImpl)
                .HandleExceptions();
        }

        private async Task<AttendingUser> LoadAttendingUserImpl()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();
            var allAttendingUsers = await this._cTimeService.GetAttendingUsers(currentUser.CompanyId, currentUser.CompanyImageAsPng);

            return allAttendingUsers.FirstOrDefault(f => f.Id == this.AttendingUserId);
        }

        private async Task SendMailImpl()
        {
            var email = new EmailMessage();
            email.To.Add(new EmailRecipient(this.AttendingUser.EmailAddress, $"{this.AttendingUser.FirstName} {this.AttendingUser.Name}"));

            await EmailManager.ShowComposeNewEmailAsync(email);
        }

        private Task CallImpl()
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1) == false)
                return Task.CompletedTask;

            PhoneCallManager.ShowPhoneCallUI(this.AttendingUser.PhoneNumber, $"{this.AttendingUser.FirstName} {this.AttendingUser.Name}");

            return Task.CompletedTask;
        }

        private async Task AddAsContactImpl()
        {
            await this._contactsService.CreateContactAsync(this.AttendingUser);

            var message = $"{this.AttendingUser.FirstName} {this.AttendingUser.Name} wurde erfolgreich als Kontakt angelegt!";
            var caption = "Kontakt angelegt!";

            await this._dialogService.ShowAsync(message, caption);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadAttendingUser.ExecuteAsync();
        }
    }
}
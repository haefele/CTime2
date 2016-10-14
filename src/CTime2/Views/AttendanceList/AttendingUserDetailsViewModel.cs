using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.AttendanceList
{
    public class AttendingUserDetailsViewModel : ReactiveScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;

        private readonly ObservableAsPropertyHelper<AttendingUser> _attendingUserHelper;
        
        public AttendingUser AttendingUser => this._attendingUserHelper.Value;

        public string AttendingUserId { get; set; }

        public UwCoreCommand<AttendingUser> LoadAttendingUser { get; }
        public UwCoreCommand<Unit> Call { get; }
        public UwCoreCommand<Unit> SendMail { get; }

        public AttendingUserDetailsViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;

            this.LoadAttendingUser = UwCoreCommand.Create(this.LoadAttendingUserImpl)
                .HandleExceptions()
                .ShowLoadingOverlay("Lade Mitarbeiter");

            this.SendMail = UwCoreCommand.Create(this.SendMailImpl);

            this.Call = UwCoreCommand.Create(this.CallImpl);

            this.LoadAttendingUser.ToProperty(this, f => f.AttendingUser, out this._attendingUserHelper);
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
            return Task.CompletedTask;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadAttendingUser.ExecuteAsync();
        }
    }
}
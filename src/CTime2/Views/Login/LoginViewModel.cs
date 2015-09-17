using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Services.CTime;
using CTime2.Services.SessionState;
using CTime2.Views.Shell;
using CTime2.Views.Shell.States;

namespace CTime2.Views.Login
{
    public class LoginViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ShellViewModel _shellViewModel;

        private string _companyId;
        private string _emailAddress;
        private string _password;

        public string CompanyId
        {
            get { return this._companyId; }
            set { this.SetProperty(ref this._companyId, value); }
        }

        public string EmailAddress
        {
            get { return this._emailAddress; }
            set { this.SetProperty(ref this._emailAddress, value); }
        }

        public string Password
        {
            get { return this._password; }
            set { this.SetProperty(ref this._password, value); }
        }

        public LoginViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ShellViewModel shellViewModel)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._shellViewModel = shellViewModel;
        }

        public async void Login()
        {
            var user = await this._cTimeService.Login(this.CompanyId, this.EmailAddress, this.Password);

            if (user != null)
            {
                this._sessionStateService.CurrentUser = user;
                this._shellViewModel.CurrentState = IoC.Get<LoggedInShellState>();
            }
        }
    }
}
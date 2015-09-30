using System;
using Caliburn.Micro;
using CTime2.Core.Extensions;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Extensions;
using CTime2.Services.Dialog;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;
using CTime2.States;
using CTime2.Views.Shell;

namespace CTime2.Views.Login
{
    public class LoginViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly ISessionStateService _sessionStateService;
        private readonly ILoadingService _loadingService;
        private readonly IApplication _application;
        private readonly IDialogService _dialogService;
        private readonly IExceptionHandler _exceptionHandler;

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

        public LoginViewModel(ICTimeService cTimeService, ISessionStateService sessionStateService, ILoadingService loadingService, IApplication application, IDialogService dialogService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._application = application;
            this._dialogService = dialogService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Anmeldung";
        }

        protected override void OnActivate()
        {
            this.CompanyId = this._sessionStateService.CompanyId;
        }

        public async void Login()
        {
            using (this._loadingService.Show("Melde an..."))
            {
                try
                {
                    var user = await this._cTimeService.Login(this.CompanyId, this.EmailAddress, this.Password);

                    if (user == null)
                    {
                        await this._dialogService.ShowAsync("Die E-Mail-Adresse oder das Passwort ist falsch.");
                        return;
                    }

                    this._sessionStateService.CompanyId = this.CompanyId;
                    this._sessionStateService.CurrentUser = user;

                    await this._sessionStateService.SaveStateAsync();

                    this._application.CurrentState = IoC.Get<LoggedInApplicationState>();
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}
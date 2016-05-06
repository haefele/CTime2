using System;
using Caliburn.Micro;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.States;
using CTime2.Strings;
using UwCore.Application;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;
using UwCore.Extensions;

namespace CTime2.Views.Login
{
    public class LoginViewModel : Screen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _sessionStateService;
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

        public LoginViewModel(ICTimeService cTimeService, IApplicationStateService sessionStateService, ILoadingService loadingService, IApplication application, IDialogService dialogService, IExceptionHandler exceptionHandler)
        {
            this._cTimeService = cTimeService;
            this._sessionStateService = sessionStateService;
            this._loadingService = loadingService;
            this._application = application;
            this._dialogService = dialogService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = CTime2Resources.Get("Navigation.Login");
        }

        protected override void OnActivate()
        {
            this.CompanyId = this._sessionStateService.GetCompanyId();
        }

        public async void Login()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.LoggingIn")))
            {
                try
                {
                    var user = await this._cTimeService.Login(this.CompanyId, this.EmailAddress, this.Password);

                    if (user == null)
                    {
                        await this._dialogService.ShowAsync(CTime2Resources.Get("Login.LoginFailed"));
                        return;
                    }

                    this._sessionStateService.SetCompanyId(this.CompanyId);
                    this._sessionStateService.SetCurrentUser(user);;

                    await this._sessionStateService.SaveStateAsync();

                    this._application.CurrentMode = IoC.Get<LoggedInApplicationState>();
                }
                catch (Exception exception)
                {
                    await this._exceptionHandler.HandleAsync(exception);
                }
            }
        }
    }
}
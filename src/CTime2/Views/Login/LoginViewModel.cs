using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.ApplicationModes;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Common;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Extensions;

namespace CTime2.Views.Login
{
    public class LoginViewModel : UwCoreScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IApplication _application;
        private readonly IDialogService _dialogService;
        private readonly IBiometricsService _biometricsService;

        private string _emailAddress;
        private string _password;
        
        public string EmailAddress
        {
            get { return this._emailAddress; }
            set { this.RaiseAndSetIfChanged(ref this._emailAddress, value); }
        }

        public string Password
        {
            get { return this._password; }
            set { this.RaiseAndSetIfChanged(ref this._password, value); }
        }

        public UwCoreCommand<Unit> Login { get; }
        public UwCoreCommand<Unit> RememberedLogin { get; }

        public LoginViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, IApplication application, IDialogService dialogService, IBiometricsService biometricsService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(application, nameof(application));
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(biometricsService, nameof(biometricsService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._application = application;
            this._dialogService = dialogService;
            this._biometricsService = biometricsService;
            
            var canLogin = this.WhenAnyValue(f => f.EmailAddress, f => f.Password, (email, password) =>
                string.IsNullOrWhiteSpace(email) == false && string.IsNullOrWhiteSpace(password) == false);
            this.Login = UwCoreCommand.Create(canLogin, this.LoginImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.LoggingIn"))
                .HandleExceptions()
                .TrackEvent("Login");
            
            var deviceAvailable = this._biometricsService.BiometricAuthDeviceIsAvailableAsync().ToObservable();
            var userAvailable = this._biometricsService.HasUserForBiometricAuthAsync().ToObservable();
            var canRememberedLogin = deviceAvailable
                .CombineLatest(userAvailable, (deviceAvail, userAvail) => deviceAvail && userAvail)
                .ObserveOnDispatcher();
            this.RememberedLogin = UwCoreCommand.Create(canRememberedLogin, this.RememberedLoginImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.LoggingIn"))
                .HandleExceptions()
                .TrackEvent("LoginRemembered");

            this.DisplayName = CTime2Resources.Get("Navigation.Login");
        }

        private async Task RememberedLoginImpl()
        {
            var user = await this._biometricsService.BiometricAuthAsync();

            if (user == null)
                return;

            await this.AfterLoginAsync(user);
        }

        private async Task LoginImpl()
        {
            var user = await this._cTimeService.Login(this.EmailAddress, this.Password);

            if (user == null)
            {
                await this._dialogService.ShowAsync(CTime2Resources.Get("Login.LoginFailed"));
                return;
            }

            await this.AfterLoginAsync(user);
        }

        private async Task AfterLoginAsync(User user)
        {
            this._applicationStateService.SetCurrentUser(user);

            await this._applicationStateService.SaveStateAsync();

            this._application.CurrentMode = IoC.Get<LoggedInApplicationMode>();
        }
    }
}
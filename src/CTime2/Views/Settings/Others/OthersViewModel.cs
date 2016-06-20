using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings.Others
{
    public class OthersViewModel : ReactiveScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;

        public ReactiveCommand<Unit> RememberLogin { get; }

        public OthersViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(biometricsService, nameof(biometricsService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;

            var canRememberLogin = new ReplaySubject<bool>(1);
            canRememberLogin.OnNext(this._applicationStateService.GetCurrentUser() != null);

            this.RememberLogin = ReactiveCommand.CreateAsyncTask(canRememberLogin, _ => this.RememberLoginImpl());
            this.RememberLogin.AttachExceptionHandler();

            this.DisplayName = CTime2Resources.Get("Navigation.Others");
        }

        private async Task RememberLoginImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();
            await this._biometricsService.RememberUserForBiometricAuthAsync(user);
        }
    }
}
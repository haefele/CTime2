using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.Biometrics;
using ReactiveUI;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace CTime2.Views.Settings.Start
{
    public class StartViewModel : ReactiveScreen
    {
        private readonly IBiometricsService _biometricsService;
        private readonly IApplicationStateService _applicationStateService;

        public ReactiveCommand<Unit> RememberLogin { get; }

        public StartViewModel(IBiometricsService biometricsService, IApplicationStateService applicationStateService)
        {
            this._biometricsService = biometricsService;
            this._applicationStateService = applicationStateService;
            this.DisplayName = "Start";

            this.RememberLogin = ReactiveCommand.CreateAsyncTask(_ => this.RememberLoginImpl());
            this.RememberLogin.AttachExceptionHandler();
        }

        private async Task RememberLoginImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();
            await this._biometricsService.RememberUserForBiometricAuthAsync(user);
        }
    }
}
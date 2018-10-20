using System;
using System.Reactive;
using System.Threading.Tasks;
using CTime2.ApplicationModes;
using CTime2.Core.Services.ApplicationState;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;

namespace CTime2.Views.HeaderDetails
{
    public class HeaderDetailsViewModel : UwCoreScreen
    {
        private readonly IDialogService _dialogService;
        private readonly IShell _shell;
        private readonly IApplicationStateService _applicationStateService;

        private readonly ObservableAsPropertyHelper<byte[]> _currentUserImageHelper;

        public byte[] CurrentUserImage => this._currentUserImageHelper.Value;

        public UwCoreCommand<byte[]> RefreshCurrentUserImage { get; }

        public HeaderDetailsViewModel(IDialogService dialogService, IShell shell, IApplicationStateService applicationStateService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(shell, nameof(shell));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            
            this._dialogService = dialogService;
            this._shell = shell;
            this._applicationStateService = applicationStateService;

            this.RefreshCurrentUserImage = UwCoreCommand.Create(this.RefreshCurrentUserImageImpl)
                .HandleExceptions();
            this.RefreshCurrentUserImage.ToProperty(this, f => f.CurrentUserImage, out this._currentUserImageHelper);

            shell.ObservableForProperty(f => f.CurrentMode)
                .InvokeCommand(this.RefreshCurrentUserImage);
        }

        private Task<byte[]> RefreshCurrentUserImageImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();
            return Task.FromResult(user?.ImageAsPng);
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            await this.RefreshCurrentUserImage.ExecuteAsync();
        }
    }
}
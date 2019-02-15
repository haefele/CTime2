using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.ApplicationModes;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Events;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;

namespace CTime2.Views.HeaderDetails
{
    [AutoSubscribeEventsForScreen]
    public class HeaderDetailsViewModel : UwCoreScreen, IHandleWithTask<UserStamped>, IHandleWithTask<ApplicationResumed>
    {
        private readonly IDialogService _dialogService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;

        private byte[] _userImage;
        private bool _isCheckedIn;

        public byte[] UserImage
        {
            get => this._userImage;
            set => this.RaiseAndSetIfChanged(ref this._userImage, value, nameof(this.UserImage));
        }
        public bool IsCheckedIn
        {
            get => this._isCheckedIn;
            set => this.RaiseAndSetIfChanged(ref this._isCheckedIn, value, nameof(this.IsCheckedIn));
        }

        public UwCoreCommand<Unit> RefreshCurrentState { get; }

        public HeaderDetailsViewModel(IDialogService dialogService, IApplicationStateService applicationStateService, ICTimeService cTimeService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            
            this._dialogService = dialogService;
            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;

            this.RefreshCurrentState = UwCoreCommand.Create(this.RefreshCurrentStateImpl)
                .HandleExceptions();
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            await this.RefreshCurrentState.ExecuteAsync();
        }

        private async Task RefreshCurrentStateImpl()
        {
            var user = this._applicationStateService.GetCurrentUser();

            this.IsCheckedIn = await this._cTimeService.IsCurrentlyCheckedIn(user.Id);
            this.UserImage = user.ImageAsPng;
        }
        
        async Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }
        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }
    }
}
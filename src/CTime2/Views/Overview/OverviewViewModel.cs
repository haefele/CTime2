using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.GeoLocation;
using CTime2.Strings;
using CTime2.Views.GeoLocationInfo;
using CTime2.Views.Overview.CheckedIn;
using CTime2.Views.Overview.CheckedOut;
using ReactiveUI;
using UwCore;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : ReactiveConductor<StampTimeStateViewModelBase>, IHandleWithTask<ApplicationResumed>, IHandleWithTask<UserStamped>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly ICTimeService _cTimeService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly INavigationService _navigationService;
        
        private string _welcomeMessage;
        private byte[] _myImage;
        private GeoLocationState _geoLocationState;
        private MyTimeViewModel _myTimeViewModel;

        public string WelcomeMessage
        {
            get { return this._welcomeMessage; }
            set { this.RaiseAndSetIfChanged(ref this._welcomeMessage, value); }
        }

        public byte[] MyImage
        {
            get { return this._myImage; }
            set { this.RaiseAndSetIfChanged(ref this._myImage, value); }
        }

        public GeoLocationState GeoLocationState
        {
            get { return this._geoLocationState; }
            set { this.RaiseAndSetIfChanged(ref this._geoLocationState, value); }
        }

        public MyTimeViewModel MyTimeViewModel
        {
            get { return this._myTimeViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._myTimeViewModel, value); }
        }

        public UwCoreCommand<Unit> RefreshCurrentState { get; }
        public UwCoreCommand<Unit> ShowGeoLocationInfo { get; }

        public OverviewViewModel(IApplicationStateService applicationStateService, ICTimeService cTimeService, IEventAggregator eventAggregator, IGeoLocationService geoLocationService, INavigationService navigationService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(geoLocationService, nameof(geoLocationService));
            Guard.NotNull(navigationService, nameof(navigationService));

            this._applicationStateService = applicationStateService;
            this._cTimeService = cTimeService;
            this._geoLocationService = geoLocationService;
            this._navigationService = navigationService;

            this.DisplayName = CTime2Resources.Get("Navigation.Overview");

            this.RefreshCurrentState = UwCoreCommand.Create(this.RefreshCurrentStateImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.CurrentState"))
                .HandleExceptions()
                .TrackEvent("RefreshCurrentState");

            this.ShowGeoLocationInfo = UwCoreCommand.Create(this.ShowGeoLocationInfoImpl)
                .HandleExceptions()
                .TrackEvent("GeoLocationInfo");

            this.MyTimeViewModel = IoC.Get<MyTimeViewModel>();
            this.MyTimeViewModel.ConductWith(this);

            eventAggregator.SubscribeScreen(this);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            var currentUser = this._applicationStateService.GetCurrentUser();

            this.WelcomeMessage = CTime2Resources.GetFormatted("Overview.WelcomeMessageFormat", currentUser.FirstName);
            this.MyImage = currentUser.ImageAsPng;

            this.GeoLocationState = await this._geoLocationService.GetGeoLocationStateAsync(currentUser);
            
            await this.RefreshCurrentState.ExecuteAsync();
        }
        
        private async Task RefreshCurrentStateImpl()
        {
            var checkedIn = await this._cTimeService.IsCurrentlyCheckedIn(this._applicationStateService.GetCurrentUser().Id);
            
            StampTimeStateViewModelBase currentState;

            if (checkedIn)
            {
                currentState = IoC.Get<CheckedInViewModel>();
            }
            else
            {
                currentState = IoC.Get<CheckedOutViewModel>();
            }

            this.ActivateItem(currentState);
        }

        private Task ShowGeoLocationInfoImpl()
        {
            this._navigationService.Popup.For<GeoLocationInfoViewModel>().Navigate();

            return Task.CompletedTask;
        }

        async Task IHandleWithTask<ApplicationResumed>.Handle(ApplicationResumed message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }

        async Task IHandleWithTask<UserStamped>.Handle(UserStamped message)
        {
            await this.RefreshCurrentState.ExecuteAsync();
        }
    }
}
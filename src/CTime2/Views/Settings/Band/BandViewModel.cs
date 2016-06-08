using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Core.Services.Band;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Extensions;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace CTime2.Views.Settings.Band
{
    public class BandViewModel : ReactiveScreen, IHandleWithTask<ApplicationResumed>
    {
        private readonly IBandService _bandService;
        private readonly ILoadingService _loadingService;
        private readonly IExceptionHandler _exceptionHandler;

        private BandViewModelState _state;

        public BandViewModelState State
        {
            get { return this._state; }
            set { this.RaiseAndSetIfChanged(ref this._state, value); }
        }

        public BandViewModel(IBandService bandService, ILoadingService loadingService, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler)
        {
            this._bandService = bandService;
            this._loadingService = loadingService;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Band";
            this.State = BandViewModelState.Loading;

            eventAggregator.Subscribe(this);
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.Band")))
            {
                await this.Reload();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            this.State = BandViewModelState.Loading;
        }

        public async void ToggleTileAsync()
        {
            try
            {
                if (this.State == BandViewModelState.NotConnected)
                    return;

                string message = this.State == BandViewModelState.Installed
                    ? CTime2Resources.Get("Loading.RemoveTileFromBand")
                    : CTime2Resources.Get("Loading.AddTileToBand");

                using (this._loadingService.Show(message))
                {
                    if (this.State == BandViewModelState.Installed)
                    {
                        await this._bandService.UnRegisterBandTileAsync();
                    }
                    else
                    {
                        await this._bandService.RegisterBandTileAsync();
                    }

                    await this.Reload();
                }
            }
            catch (Exception exception)
            {
                await this._exceptionHandler.HandleAsync(exception);
            }
        }
        
        private async Task Reload()
        {
            try
            {
                var bandInfo = await this._bandService.GetBand();

                if (bandInfo == null)
                {
                    this.State = BandViewModelState.NotConnected;
                    return;
                }

                if (await this._bandService.IsBandTileRegisteredAsync())
                {
                    this.State = BandViewModelState.Installed;
                }
                else
                {
                    this.State = BandViewModelState.NotInstalled;
                }
            }
            catch (Exception exception)
            {
                await this._exceptionHandler.HandleAsync(exception);
            }
        }

        public async Task Handle(ApplicationResumed message)
        {
            using (this._loadingService.Show(CTime2Resources.Get("Loading.Band")))
            {
                await this.Reload();
            }
        }
    }
}
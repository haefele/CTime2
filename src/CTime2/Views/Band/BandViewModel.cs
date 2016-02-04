using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Services.Band;
using CTime2.Events;
using CTime2.Extensions;
using CTime2.Services.ExceptionHandler;
using CTime2.Services.Loading;

namespace CTime2.Views.Band
{
    public class BandViewModel : Screen, IHandleWithTask<ApplicationResumedEvent>
    {
        private readonly IBandService _bandService;
        private readonly ILoadingService _loadingService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExceptionHandler _exceptionHandler;

        private bool _isBandTileRegistered;
        private bool _isConnectedWithTile;
        private string _bandName;

        public string BandName
        {
            get { return this._bandName; }
            set { this.SetProperty(ref this._bandName, value); }
        }

        public bool IsBandTileRegistered
        {
            get { return this._isBandTileRegistered; }
            set { this.SetProperty(ref this._isBandTileRegistered, value); }
        }

        public bool IsConnectedWithTile
        {
            get { return this._isConnectedWithTile; }
            set { this.SetProperty(ref this._isConnectedWithTile, value); }
        }

        public BandViewModel(IBandService bandService, ILoadingService loadingService, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler)
        {
            this._bandService = bandService;
            this._loadingService = loadingService;
            this._eventAggregator = eventAggregator;
            this._exceptionHandler = exceptionHandler;

            this.DisplayName = "Microsoft Band";

            this._eventAggregator.Subscribe(this);
        }

        protected override async void OnActivate()
        {
            using (this._loadingService.Show("Lade"))
            {
                await this.Reload();
            }
        }

        public async void ToggleTileAsync()
        {
            try
            {
                string message = this.IsBandTileRegistered
                    ? "Entferne Tile"
                    : "Registriere Tile";

                using (this._loadingService.Show(message))
                {
                    if (this.IsBandTileRegistered)
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
        
        public async void ToggleTileConnectionAsync()
        {
            try
            {
                using (this._loadingService.Show("Verbinde mit Tile"))
                {
                    if (this.IsConnectedWithTile)
                    {
                        await this._bandService.DisconnectFromTileAsync();
                    }
                    else
                    {
                        await this._bandService.ConnectWithTileAsync();
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
                this.BandName = string.Empty;
                this.IsBandTileRegistered = false;
                this.IsConnectedWithTile = false;

                var bandInfo = await this._bandService.GetBand();

                this.BandName = bandInfo?.Name;
                this.IsBandTileRegistered = await this._bandService.IsBandTileRegisteredAsync();
                this.IsConnectedWithTile = await this._bandService.IsConnectedWithTile();
            }
            catch (Exception exception)
            {
                await this._exceptionHandler.HandleAsync(exception);
            }
        }

        public async Task Handle(ApplicationResumedEvent message)
        {
            using (this._loadingService.Show("Lade"))
            {
                await this.Reload();
            }
        }
    }
}
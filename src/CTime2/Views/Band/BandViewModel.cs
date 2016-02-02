using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Services.Band;
using CTime2.Extensions;
using CTime2.Services.Loading;
using Microsoft.Band;
using Microsoft.Band.Notifications;

namespace CTime2.Views.Band
{
    public class BandViewModel : Screen
    {
        private readonly IBandService _bandService;
        private readonly ILoadingService _loadingService;

        private bool _isBandTileRegistered;
        private bool _isConnectedWithTile;

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

        public BandViewModel(IBandService bandService, ILoadingService loadingService)
        {
            this._bandService = bandService;
            this._loadingService = loadingService;
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
                using (this._loadingService.Show("Registriere"))
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
            catch (CTimeException exception)
            {


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
            catch (CTimeException exception)
            {
                
            }
        }

        private async Task Reload()
        {
            try
            {
                this.IsBandTileRegistered = false;
                this.IsConnectedWithTile = false;

                this.IsBandTileRegistered = await this._bandService.IsBandTileRegisteredAsync();
                this.IsConnectedWithTile = await this._bandService.IsConnectedWithTile();
            }
            catch (CTimeException exception)
            {

            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Services.Band;
using CTime2.Extensions;
using CTime2.Services.Loading;

namespace CTime2.Views.Band
{
    public class BandViewModel : Screen
    {
        private readonly IBandService _bandService;
        private readonly ILoadingService _loadingService;
        private bool _isBandConnected;
        private bool _isBandTileInstalled;

        public bool IsBandConnected
        {
            get { return this._isBandConnected; }
            set { this.SetProperty(ref this._isBandConnected, value); }
        }

        public bool IsBandTileInstalled
        {
            get { return this._isBandTileInstalled; }
            set { this.SetProperty(ref this._isBandTileInstalled, value); }
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
                    if (this.IsBandTileInstalled)
                        await this._bandService.UnRegisterBandTileAsync();
                    else
                        await this._bandService.RegisterBandTileAsync();

                    await this.Reload();
                }
            }
            catch (CTimeException exception)
            {


            }
        }

        public async void ConnectWithTile()
        {
            try
            {
                using (this._loadingService.Show("Verbinde mit Tile"))
                {
                    await this._bandService.ConnectWithBandAsync();
                }
            }
            catch (CTimeException exception)
            {


            }
        }

        public async void DisconnectFromTile()
        {
            try
            {
                using (this._loadingService.Show("Trenne vom Tile"))
                {
                    await this._bandService.DisconnectFromBandAsync();
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
                this.IsBandConnected = false;
                this.IsBandTileInstalled = false;

                this.IsBandConnected = await this._bandService.IsBandConnectedAsync();
                this.IsBandTileInstalled = await this._bandService.IsBandTileRegisteredAsync();
            }
            catch (CTimeException exception)
            {

            }
        }
    }
}
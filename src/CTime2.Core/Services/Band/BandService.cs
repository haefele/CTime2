﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Media.Imaging;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Services.Band.Pages;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace CTime2.Core.Services.Band
{
    public class BandService : IBandService, ICTimeStampHelperCallback
    {
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        
        public BandService(ISessionStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;

            BackgroundTileEventHandler.Instance.TileOpened += this.OnTileOpened;
            BackgroundTileEventHandler.Instance.TileButtonPressed += this.OnTileButtonPressed;
            BackgroundTileEventHandler.Instance.TileClosed += this.OnTileClosed;
        }

        public async Task<BandInfo> GetBand()
        {
            var bandInfos = await BandClientManager.Instance.GetBandsAsync();

            if (bandInfos.Any() == false)
                return null;

            return new BandInfo
            {
                Name = bandInfos.First().Name
            };
        }

        
        public async Task<bool> IsBandTileRegisteredAsync()
        {
            using (var client = await this.GetClientAsync(false))
            {
                return await this.IsBandTileRegisteredInternalAsync(client);
            }
        }

        public async Task RegisterBandTileAsync()
        {
            using (var client = await this.GetClientAsync(false))
            {
                await this.RegisterBandTileInternalAsync(client);
            }
        }

        public async Task UnRegisterBandTileAsync()
        {
            using (var client = await this.GetClientAsync(false))
            {
                await this.UnRegisterBandTileInternalAsync(client);
            }
        }


        private TaskCompletionSource<object> _backgroundTileEventTaskSource; 
        public async Task HandleTileEventAsync(ValueSet message)
        {
            //Do nothing if we are currently handling a tile event
            if (this._backgroundTileEventTaskSource != null)
                return;

            this._backgroundTileEventTaskSource = new TaskCompletionSource<object>();
            bool eventHandled = BackgroundTileEventHandler.Instance.HandleTileEvent(message);

            if (eventHandled == false)
                this._backgroundTileEventTaskSource.SetResult(null);

            await this._backgroundTileEventTaskSource.Task;
            this._backgroundTileEventTaskSource = null;
        }


        private async Task<bool> IsBandTileRegisteredInternalAsync(IBandClient client)
        {
            var tiles = await client.TileManager.GetTilesAsync();
            return tiles.Any(f => f.TileId == BandConstants.TileId);
        }

        private async Task RegisterBandTileInternalAsync(IBandClient client)
        {
            await this.UnRegisterBandTileInternalAsync(client);
            
            var bandHardwareVersion = int.Parse(await client.GetHardwareVersionAsync());

            if (bandHardwareVersion < 20)
                throw new CTimeException("Leider werden nur Microsoft Band 2 unterstützt.");
            
            var availableTileCount = await client.TileManager.GetRemainingTileCapacityAsync();

            if (availableTileCount == 0)
                throw new CTimeException("Auf dem Band ist kein Platz mehr für weitere Tiles.");
            
            var tile = new BandTile(BandConstants.TileId)
            {
                SmallIcon = new WriteableBitmap(24, 24).ToBandIcon(),
                TileIcon = new WriteableBitmap(48, 48).ToBandIcon(),
                Name = "c-time"
            };

            var stampPageLayout = new StampPageLayout();
            tile.PageLayouts.Add(stampPageLayout.Layout);

            var startPageLayout = new StartPageLayout();
            tile.PageLayouts.Add(startPageLayout.Layout);
            
            await client.TileManager.AddTileAsync(tile);
            
            await client.TileManager.SetPagesAsync(BandConstants.TileId,
                new PageData(BandConstants.StartPageId, 0, startPageLayout.Data.All));

            await client.TileManager.SetPagesAsync(BandConstants.TileId,
                new PageData(BandConstants.StampPageId, 1, stampPageLayout.Data.All));

            await client.SubscribeToBackgroundTileEventsAsync(tile.TileId);
        }

        private async Task UnRegisterBandTileInternalAsync(IBandClient client)
        {
            await client.TileManager.RemoveTileAsync(BandConstants.TileId);
        }
        

        private async Task<IBandClient> GetClientAsync(bool isBackground)
        {
            var bandInfos = await BandClientManager.Instance.GetBandsAsync(isBackground);

            if (bandInfos.Any() == false)
                throw new CTimeException();
            
            return await BandClientManager.Instance.ConnectAsync(bandInfos.First());
        }
        
        
        private IBandClient _backgroundTileClient;
        private async void OnTileOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> bandTileEventArgs)
        {
            try
            {
                this._backgroundTileClient = await this.GetClientAsync(true);
            }
            finally
            {
                this._backgroundTileEventTaskSource.SetResult(null);
            }
        }

        private void OnTileClosed(object sender, BandTileEventArgs<IBandTileClosedEvent> e)
        {
            try
            {
                this._backgroundTileClient?.Dispose();
            }
            finally
            {
                this._backgroundTileEventTaskSource.SetResult(null);
            }
        }

        private async void OnTileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            try
            {
                if (e.TileEvent.TileId == BandConstants.TileId)
                {
                    //if (e.TileEvent.ElementId == BandConstants.StampElementId)
                    //{
                    //    var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                    //    bool checkedIn = currentTime != null && currentTime.State.IsEntered();

                    //    var stampHelper = new CTimeStampHelper(this._sessionStateService, this._cTimeService);
                    //    await stampHelper.Stamp(this, checkedIn ? TimeState.Left : TimeState.Entered);

                    //    await this.UpdateTileContentAsync(this._backgroundTileClient);
                    //}
                    //else if (e.TileEvent.ElementId == BandConstants.TestElementId)
                    //{
                    //    await this._backgroundTileClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
                    //}
                }
            }
            finally
            {
                this._backgroundTileEventTaskSource.SetResult(null);
            }
        }

        #region Callbacks
        public async Task OnNotLoggedIn()
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Nicht eingeloggt.");
        }

        public bool SupportsQuestions()
        {
            return false;
        }

        public Task OnDidNothing()
        {
            throw new NotImplementedException();
        }

        public Task<bool> OnAlreadyCheckedInWannaCheckOut()
        {
            throw new NotImplementedException();
        }

        public async Task OnAlreadyCheckedIn()
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits eingestempelt.");
        }

        public Task<bool> OnAlreadyCheckedOutWannaCheckIn()
        {
            throw new NotImplementedException();
        }

        public async Task OnAlreadyCheckedOut()
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits ausgestempelt.");
        }

        public async Task OnSuccess(TimeState timeState)
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Erfolgreich gestempelt.");
        }
        #endregion
    }
}
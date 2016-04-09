using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Media.Imaging;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Extensions;
using CTime2.Core.Logging;
using CTime2.Core.Services.Band.Pages;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using CTime2.Core.Strings;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace CTime2.Core.Services.Band
{
    public class BandService : IBandService, ICTimeStampHelperCallback
    {
        #region Logging
        private static readonly Logger Logger = LoggerFactory.GetLogger<CTimeService>();
        #endregion

        #region Fields
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        #endregion

        #region Constructors
        public BandService(ISessionStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;

            BackgroundTileEventHandler.Instance.TileOpened += this.OnTileOpened;
            BackgroundTileEventHandler.Instance.TileButtonPressed += this.OnTileButtonPressed;
            BackgroundTileEventHandler.Instance.TileClosed += this.OnTileClosed;
        }
        #endregion

        #region Foreground Methods
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
                throw new CTimeException(CTime2CoreResources.Get("BandService.OnlyBand2Supported"));

            var availableTileCount = await client.TileManager.GetRemainingTileCapacityAsync();

            if (availableTileCount == 0)
                throw new CTimeException(CTime2CoreResources.Get("BandService.NoSpaceForBandTile"));

            var tile = new BandTile(BandConstants.TileId)
            {
                SmallIcon = new WriteableBitmap(24, 24).ToBandIcon(),
                TileIcon = new WriteableBitmap(48, 48).ToBandIcon(),
                Name = "c-time"
            };

            var testConnectionPageLayout = new TestConnectionPageLayout();
            tile.PageLayouts.Add(testConnectionPageLayout.Layout);

            var stampPageLayout = new StampPageLayout();
            tile.PageLayouts.Add(stampPageLayout.Layout);

            var startPageLayout = new StartPageLayout();
            tile.PageLayouts.Add(startPageLayout.Layout);

            await client.TileManager.AddTileAsync(tile);

            await this.ChangeTileDataToLoadingAsync(client);

            await client.SubscribeToBackgroundTileEventsAsync(tile.TileId);
        }

        private async Task UnRegisterBandTileInternalAsync(IBandClient client)
        {
            await client.TileManager.RemoveTileAsync(BandConstants.TileId);
        }
        #endregion

        #region Tile Event Methods
        private TaskCompletionSource<object> _backgroundTileEventTaskSource; 

        public async Task HandleTileEventAsync(ValueSet message)
        {
            //Check if we are currently handling another tile event
            if (this._backgroundTileEventTaskSource != null)
            {
                object eventType;

                if (message.TryGetValue("Type", out eventType) && 
                    eventType as string == "TileButtonPressedEvent")
                {
                    //If its a tile button pressed event, we do nothing
                    return;
                }
                else
                {
                    //For other events we wait till it has finished
                    await this._backgroundTileEventTaskSource.Task;
                }
            }

            this._backgroundTileEventTaskSource = new TaskCompletionSource<object>();
            bool eventHandled = BackgroundTileEventHandler.Instance.HandleTileEvent(message);

            if (eventHandled == false)
                this._backgroundTileEventTaskSource.SetResult(null);

            await this._backgroundTileEventTaskSource.Task;
            this._backgroundTileEventTaskSource = null;
        }

        private IBandClient _backgroundTileClient;

        private async void OnTileOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> bandTileEventArgs)
        {
            try
            {
                Logger.Debug("Handling TileOpened event.");

                this._backgroundTileClient = await this.GetClientAsync(true);
                await this.ChangeTileDataToReadyAsync(this._backgroundTileClient);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while handling TileOpened event.");
            }
            finally
            {
                this._backgroundTileEventTaskSource.SetResult(null);
            }
        }

        private async void OnTileClosed(object sender, BandTileEventArgs<IBandTileClosedEvent> e)
        {
            try
            {
                Logger.Debug("Handling TileClosed event.");

                //Make sure the client is created
                //The background task might be stopped after the OnTileOpened event
                if (this._backgroundTileClient == null)
                    this._backgroundTileClient = await this.GetClientAsync(true);

                await this.ChangeTileDataToLoadingAsync(this._backgroundTileClient);
                this._backgroundTileClient.Dispose();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while handling TileClosed event.");
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
                Logger.Debug("Handling TileButtonPressed event.");

                //Make sure the client is created
                //The background task might be stopped after the OnTileOpened event
                if (this._backgroundTileClient == null)
                    this._backgroundTileClient = await this.GetClientAsync(true);

                if (e.TileEvent.TileId == BandConstants.TileId)
                {
                    if (e.TileEvent.ElementId == new StampPageLayout().StampTextButton.ElementId)
                    {
                        await this._backgroundTileClient.NotificationManager.VibrateAsync(VibrationType.NotificationOneTone);

                        var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                        bool checkedIn = currentTime != null && currentTime.State.IsEntered();

                        var stampHelper = new CTimeStampHelper(this._sessionStateService, this._cTimeService);
                        await stampHelper.Stamp(this, checkedIn ? TimeState.Left : TimeState.Entered);

                        await this.ChangeTileDataToReadyAsync(this._backgroundTileClient);
                    }
                    else if (e.TileEvent.ElementId == new TestConnectionPageLayout().TestTextButton.ElementId)
                    {
                        await this._backgroundTileClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while handling TileButtonPressed event.");

                if (this._backgroundTileClient != null)
                {
                    var title = CTime2CoreResources.Get("BandService.ApplicationName");
                    var body = CTime2CoreResources.GetFormatted("BandService.ErrorOccurred", Environment.NewLine, exception.GetFullMessage());

                    await this._backgroundTileClient.NotificationManager.ShowDialogAsync(BandConstants.TileId, title, body);
                }
            }
            finally
            {
                this._backgroundTileEventTaskSource.SetResult(null);
            }
        }
        #endregion

        #region Private Methods
        private async Task<IBandClient> GetClientAsync(bool isBackground)
        {
            var bandInfos = await BandClientManager.Instance.GetBandsAsync(isBackground);

            if (bandInfos.Any() == false)
                throw new CTimeException();
            
            return await BandClientManager.Instance.ConnectAsync(bandInfos.First());
        }
        #endregion

        #region Tile Data Methods
        private async Task ChangeTileDataToLoadingAsync(IBandClient client)
        {
            var startPageLayout = new StartPageLayout
            {
                CTimeTextBlockData = { Text = CTime2CoreResources.Get("BandService.ApplicationName") },
                LoadingTextBlockData = { Text = CTime2CoreResources.Get("BandService.Loading") },
                PleaseWaitTextBlockData = { Text = CTime2CoreResources.Get("BandService.PleaseWait") },
            };
            var stampPageLayout = new StampPageLayout
            {
                StampTextBlockData = {Text = CTime2CoreResources.Get("BandService.Stamp") },
                StampTextButtonData = {Text = string.Empty}
            };
            var testConnectionPageLayout = new TestConnectionPageLayout
            {
                ConnectionTextBlockData = { Text = CTime2CoreResources.Get("BandService.Connection") },
                TestTextButtonData = { Text = string.Empty }
            };

            await this.ChangeTileData(client, startPageLayout, stampPageLayout, testConnectionPageLayout);
        }

        private async Task ChangeTileDataToReadyAsync(IBandClient client)
        {
            var currentState = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
            bool checkedIn = currentState != null && currentState.State.IsEntered();


            var startPageLayout = new StartPageLayout
            {
                CTimeTextBlockData = { Text = CTime2CoreResources.Get("BandService.ApplicationName") },
                LoadingTextBlockData = { Text = CTime2CoreResources.Get("BandService.Ready") },
                PleaseWaitTextBlockData = { Text = string.Empty },
            };
            var stampPageLayout = new StampPageLayout
            {
                StampTextBlockData = { Text = CTime2CoreResources.Get("BandService.Stamp") },
                StampTextButtonData =
                {
                    Text = checkedIn 
                        ? CTime2CoreResources.Get("BandService.CheckOut") 
                        : CTime2CoreResources.Get("BandService.CheckIn")
                }
            };
            var testConnectionPageLayout = new TestConnectionPageLayout
            {
                ConnectionTextBlockData = {Text = CTime2CoreResources.Get("BandService.Connection") },
                TestTextButtonData = {Text = CTime2CoreResources.Get("BandService.Test") }
            };

            await this.ChangeTileData(client, startPageLayout, stampPageLayout, testConnectionPageLayout);
        }

        private async Task ChangeTileData(IBandClient client, StartPageLayout startPage, StampPageLayout stampPage, TestConnectionPageLayout testConnectionPage)
        {
            await client.TileManager.SetPagesAsync(BandConstants.TileId,
                new PageData(BandConstants.TestConnectionPageId, 0, testConnectionPage.Data.All),
                new PageData(BandConstants.StampPageId, 1, stampPage.Data.All),
                new PageData(BandConstants.StartPageId, 2, startPage.Data.All));
        }
        #endregion

        #region Callbacks
        public async Task OnNotLoggedIn()
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(
                BandConstants.TileId,
                CTime2CoreResources.Get("BandService.ApplicationName"),
                CTime2CoreResources.Get("BandService.NotLoggedIn"));
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
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(
                BandConstants.TileId,
                CTime2CoreResources.Get("BandService.ApplicationName"),
                CTime2CoreResources.Get("BandService.AlreadyCheckedIn"));
        }

        public Task<bool> OnAlreadyCheckedOutWannaCheckIn()
        {
            throw new NotImplementedException();
        }

        public async Task OnAlreadyCheckedOut()
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(
                BandConstants.TileId,
                CTime2CoreResources.Get("BandService.ApplicationName"),
                CTime2CoreResources.Get("BandService.AlreadyCheckedOut"));
        }

        public async Task OnSuccess(TimeState timeState)
        {
            await this._backgroundTileClient.NotificationManager.ShowDialogAsync(
                BandConstants.TileId,
                CTime2CoreResources.Get("BandService.ApplicationName"),
                CTime2CoreResources.Get("BandService.StampedSuccessfully"));
        }
        #endregion
    }
}
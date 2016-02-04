using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using CTime2.Core.Common;
using CTime2.Core.Data;
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

        private IBandClient _bandClient;

        private bool _isConnectedWithTile;
        private bool _isExecutingAButton;

        public BandService(ISessionStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
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
            var client = await this.GetClientAsync();
            
            var tiles = await client.TileManager.GetTilesAsync();
            return tiles.Any(f => f.TileId == BandConstants.TileId);
        }

        public async Task RegisterBandTileAsync()
        {
            await this.UnRegisterBandTileAsync();

            var client = await this.GetClientAsync();
            var tile = new BandTile(BandConstants.TileId)
            {
                SmallIcon = new WriteableBitmap(24, 24).ToBandIcon(),
                TileIcon = new WriteableBitmap(48, 48).ToBandIcon(),
                Name = "c-time",
                PageLayouts =
                {
                    new PageLayout
                    {
                        Root = new FilledPanel
                        {
                            Rect = new PageRect(0, 0, 258, 128),
                            Elements =
                            {
                                new TextBlock
                                {
                                    ElementId = BandConstants.HeaderElementId,
                                    ColorSource = ElementColorSource.BandHighlight,
                                    AutoWidth = true,
                                    Rect = new PageRect(15, 0, 258, 30)
                                },
                                new TextButton
                                {
                                    PressedColor = new BandColor(0, 255, 0),
                                    ElementId = BandConstants.StampElementId,
                                    Rect = new PageRect(15, 40, 258, 40),
                                },
                                new TextButton
                                {
                                    PressedColor = new BandColor(255, 255, 0),
                                    ElementId = BandConstants.TestElementId,
                                    Rect = new PageRect(15, 86, 258, 40),
                                }
                            }
                        }
                    }
                }
            };

            await client.TileManager.AddTileAsync(tile);
        }

        public async Task UnRegisterBandTileAsync()
        {
            var client = await this.GetClientAsync();
            await client.TileManager.RemoveTileAsync(BandConstants.TileId);
        }
        

        public Task<bool> IsConnectedWithTile()
        {
            return Task.FromResult(this._isConnectedWithTile);
        }

        public async Task ConnectWithTileAsync()
        {
            await this.DisconnectFromTileAsync();

            try
            {
                var client = await this.GetClientAsync();

                client.TileManager.TileOpened += this.TileManagerOnTileOpened;
                client.TileManager.TileButtonPressed += this.TileManagerOnTileButtonPressed;

                await client.TileManager.StartReadingsAsync();

                this._isConnectedWithTile = true;
            }
            catch
            {
                await this.DisconnectFromTileAsync();

                this._isConnectedWithTile = false;
            }
        }

        public async Task DisconnectFromTileAsync()
        {
            var client = await this.GetClientAsync();

            await client.TileManager.StopReadingsAsync();

            client.TileManager.TileOpened -= this.TileManagerOnTileOpened;
            client.TileManager.TileButtonPressed -= this.TileManagerOnTileButtonPressed;
            
            this._isConnectedWithTile = false;
        }


        public async Task DisconnectFromBandAsync()
        {
            if (this._bandClient != null)
            {
                if (await this.IsConnectedWithTile())
                    await this.DisconnectFromTileAsync();

                this._bandClient.Dispose();
                this._bandClient = null;
            }
        }
        

        private async Task<IBandClient> GetClientAsync()
        {
            if (this._bandClient != null)
            {
                //Just check if the band client still works
                await this._bandClient.GetHardwareVersionAsync();

                return this._bandClient;
            }
            
            var bandInfos = await BandClientManager.Instance.GetBandsAsync();

            if (bandInfos.Any() == false)
                throw new CTimeException();

            this._bandClient = await BandClientManager.Instance.ConnectAsync(bandInfos.First());
            return this._bandClient;
        }

        private async Task UpdateTileContentAsync()
        {
            var client = await this.GetClientAsync();

            var currentState = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
            bool checkedIn = currentState != null && currentState.State.IsEntered();

            var pageData = new PageData(BandConstants.TileId, 0,
                new TextBlockData(BandConstants.HeaderElementId, "c-time"),
                new TextButtonData(BandConstants.StampElementId, checkedIn ? "Check-out" : "Check-in"),
                new TextButtonData(BandConstants.TestElementId, "Test"));

            await client.TileManager.SetPagesAsync(BandConstants.TileId, pageData);
        }

        #region Band Tile Events
        private async void TileManagerOnTileOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> e)
        {
            if (e.TileEvent.TileId == BandConstants.TileId)
            {
                await this.UpdateTileContentAsync();
            }
        }

        private async void TileManagerOnTileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            var bandClient = (IBandClient)sender;

            if (e.TileEvent.TileId == BandConstants.TileId)
            {
                if (this._isExecutingAButton)
                    return;

                this._isExecutingAButton = true;

                if (e.TileEvent.ElementId == BandConstants.StampElementId)
                {
                    var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
                    bool checkedIn = currentTime != null && currentTime.State.IsEntered();

                    var stampHelper = new CTimeStampHelper(this._sessionStateService, this._cTimeService);
                    await stampHelper.Stamp(this, checkedIn ? TimeState.Left : TimeState.Entered);

                    await this.UpdateTileContentAsync();
                }
                else if (e.TileEvent.ElementId == BandConstants.TestElementId)
                {
                    await bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
                }

                this._isExecutingAButton = false;
            }
        }
        #endregion

        #region Callbacks
        public async Task OnNotLoggedIn()
        {
            var client = await this.GetClientAsync();
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Nicht eingeloggt.");
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
            var client = await this.GetClientAsync();
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits eingestempelt.");
        }

        public Task<bool> OnAlreadyCheckedOutWannaCheckIn()
        {
            throw new NotImplementedException();
        }

        public async Task OnAlreadyCheckedOut()
        {
            var client = await this.GetClientAsync();
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Bereits ausgestempelt.");
        }

        public async Task OnSuccess(TimeState timeState)
        {
            var client = await this.GetClientAsync();
            await client.NotificationManager.ShowDialogAsync(BandConstants.TileId, "c-time", "Erfolgreich gestempelt.");
        }
        #endregion
    }
}
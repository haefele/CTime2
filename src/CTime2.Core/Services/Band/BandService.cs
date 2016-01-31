using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Media.Imaging;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.SessionState;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace CTime2.Core.Services.Band
{
    public class BandService : IBandService
    {
        private readonly ISessionStateService _sessionStateService;
        private readonly ICTimeService _cTimeService;
        private IBandInfo _bandInfo;
        private IBandClient _client;

        public BandService(ISessionStateService sessionStateService, ICTimeService cTimeService)
        {
            this._sessionStateService = sessionStateService;
            this._cTimeService = cTimeService;
        }

        public async Task RegisterBandTileAsync()
        {
            await this.InitializeAsync();
            await this.RemoveExistingTileAsync();

            var tile = new BandTile(BandConstants.TileId)
            {
                SmallIcon = new WriteableBitmap(24, 24).ToBandIcon(),
                TileIcon = new WriteableBitmap(48, 48).ToBandIcon(),
                Name = "c-time",
                PageLayouts =
                {
                    this.CreatePageLayout()
                }
            };

            await this._client.TileManager.AddTileAsync(tile);
            await this._client.TileManager.SetPagesAsync(BandConstants.TileId, this.CreatePageData());
        }

        public async Task StartListeningForEvents()
        {
            await this.InitializeAsync();

            this._client.TileManager.TileButtonPressed += async (sender, args) =>
            {
                if (args.TileEvent.TileId == BandConstants.TileId)
                {
                    if (args.TileEvent.ElementId == BandConstants.CheckInElementId)
                    {
                        await this.Stamp(TimeState.Entered);
                    }
                    if (args.TileEvent.ElementId == BandConstants.CheckOutElementId)
                    {
                        await this.Stamp(TimeState.Left);
                    }
                }
            };

            await this._client.TileManager.StartReadingsAsync();
        }
        
        private PageLayout CreatePageLayout()
        {
            var header = new TextBlock
            {
                ElementId = BandConstants.HeaderElementId,
                ColorSource = ElementColorSource.BandHighlight,
                AutoWidth = true,
                Rect = new PageRect(15, 0, 258, 30)
            };
            var checkInButton = new TextButton
            {
                PressedColor = new BandColor(0, 255, 0),
                ElementId = BandConstants.CheckInElementId,
                Rect = new PageRect(15, 40, 258, 40),
            };
            var checkOutButton = new TextButton
            {
                PressedColor = new BandColor(255, 0, 0),
                ElementId = BandConstants.CheckOutElementId,
                Rect = new PageRect(15, 86, 258, 40),
            };

            var panel = new FilledPanel(header, checkInButton, checkOutButton)
            {
                Rect = new PageRect(0, 0, 258, 128),
            };

            return new PageLayout(panel);
        }

        private PageData CreatePageData()
        {
            return new PageData(BandConstants.TileId, 0,
                new TextBlockData(BandConstants.HeaderElementId, "c-time"),
                new TextButtonData(BandConstants.CheckInElementId, "Checkin"),
                new TextButtonData(BandConstants.CheckOutElementId, "Checkout"));
        }

        private async Task InitializeAsync()
        {
            if (this._client == null)
            {
                var bandInfos = await BandClientManager.Instance.GetBandsAsync();
                this._bandInfo = bandInfos[0];
                this._client = await BandClientManager.Instance.ConnectAsync(this._bandInfo);
            }
        }
        
        private async Task RemoveExistingTileAsync()
        {
            await this.InitializeAsync();

            var tiles = await this._client.TileManager.GetTilesAsync();
            foreach (var tileToRemove in tiles)
            {
                if (tileToRemove.TileId == BandConstants.TileId)
                    await this._client.TileManager.RemoveTileAsync(tileToRemove);
            }
        }

        private async Task Stamp(TimeState timeState)
        {
            if (this._sessionStateService.CurrentUser == null)
            {
                await this.ShowMessage("Ups", "Nicht eingeloggt.");
                return;
            }
            
            var currentTime = await this._cTimeService.GetCurrentTime(this._sessionStateService.CurrentUser.Id);
            bool checkedIn = currentTime != null && currentTime.State.IsEntered();

            if (checkedIn && timeState.IsEntered())
            {
                await this.ShowMessage("Ups", "Bereits eingestempelt.");
                return;
            }

            if (checkedIn == false && timeState.IsLeft())
            {
                await this.ShowMessage("Ups", "Bereits ausgestempelt.");
                return;
            }

            if (timeState.IsLeft())
            {
                if (currentTime.State.IsTrip())
                {
                    timeState = timeState | TimeState.Trip;
                }

                if (currentTime.State.IsHomeOffice())
                {
                    timeState = timeState | TimeState.HomeOffice;
                }
            }
            
            //await this._cTimeService.SaveTimer(
            //    this._sessionStateService.CurrentUser.Id, 
            //    DateTime.Now, 
            //    this._sessionStateService.CompanyId, 
            //    timeState);

            await this.ShowMessage("Yeah", "Erfolgreich gestempelt.");
        }
        private async Task ShowMessage(string title, string message)
        {
            await this.InitializeAsync();

            await this._client.NotificationManager.ShowDialogAsync(BandConstants.TileId, title, message);
        }
    }
}
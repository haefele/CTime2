using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace CTime2.Core.Services.Band
{
    public class BandService : IBandService
    {
        private IBandClient _client;

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

            this._client.TileManager.TileButtonPressed += (sender, args) =>
            {
                if (args.TileEvent.TileId == BandConstants.TileId)
                {
                    if (args.TileEvent.ElementId == BandConstants.CheckInElementId)
                    {
                        this.CheckInPressed?.Invoke(this, EventArgs.Empty);
                    }
                    if (args.TileEvent.ElementId == BandConstants.CheckOutElementId)
                    {
                        this.CheckOutPressed?.Invoke(this, EventArgs.Empty);
                    }
                }
            };

            await this._client.TileManager.StartReadingsAsync();
        }

        public async Task ShowMessage(string title, string message)
        {
            await this.InitializeAsync();
            
            await this._client.NotificationManager.ShowDialogAsync(BandConstants.TileId, title, message);
        }
        
        public event EventHandler CheckInPressed;
        public event EventHandler CheckOutPressed;

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
                this._client = await BandClientManager.Instance.ConnectAsync(bandInfos[0]);
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
    }
}
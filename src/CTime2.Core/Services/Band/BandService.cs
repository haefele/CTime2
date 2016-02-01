using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
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
    public class BandService : IBandService
    {
        public async Task<bool> IsBandConnectedAsync()
        {
            var bandInfos = await BandClientManager.Instance.GetBandsAsync();
            return bandInfos.Any();
        }


        public async Task<bool> IsBandTileRegisteredAsync()
        {
            using (var client = await this.GetClientAsync())
            {
                var tiles = await client.TileManager.GetTilesAsync();
                return tiles.Any(f => f.TileId == BandConstants.TileId);
            }
        }

        public async Task RegisterBandTileAsync()
        {
            await this.UnRegisterBandTileAsync();

            using (var client = await this.GetClientAsync())
            {
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

                await client.TileManager.AddTileAsync(tile);
                await client.TileManager.SetPagesAsync(BandConstants.TileId, this.CreatePageData());
            }
        }

        public async Task UnRegisterBandTileAsync()
        {
            using (var client = await this.GetClientAsync())
            {
                await client.TileManager.RemoveTileAsync(BandConstants.TileId);
            }
        }


        public Task<bool> IsConnectedWithBandAsync()
        {
            var registrationPair = BackgroundTaskRegistration.AllTasks.FirstOrDefault(f => f.Value.Name == BandConstants.BackgroundTaskId);
            return Task.FromResult(registrationPair.Value != null);
        }

        public async Task ConnectWithBandAsync()
        {
            await this.DisconnectFromBandAsync();

            var access = await BackgroundExecutionManager.RequestAccessAsync();

            if (access == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                access == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = BandConstants.BackgroundTaskId,
                    TaskEntryPoint = BandConstants.BackgroundTaskEntryPoint
                };

                var trigger = new DeviceUseTrigger();
                builder.SetTrigger(trigger);
                builder.Register();

                var bandDeviceId = await this.FindBandDeviceIdAsync();
                var triggerResult = await trigger.RequestAsync(bandDeviceId);

                switch (triggerResult)
                {
                    case DeviceTriggerResult.Allowed:
                        break;
                    case DeviceTriggerResult.DeniedByUser:
                        throw new CTimeException();
                    case DeviceTriggerResult.DeniedBySystem:
                        throw new CTimeException();
                    case DeviceTriggerResult.LowBattery:
                        throw new CTimeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Task DisconnectFromBandAsync()
        {
            var registrationPair = BackgroundTaskRegistration.AllTasks.FirstOrDefault(f => f.Value.Name == BandConstants.BackgroundTaskId);
            registrationPair.Value?.Unregister(true);

            return Task.CompletedTask;
        }


        public async Task<IDisposable> ListenForEventsAsync(Action onCheckIn, Action onCheckOut)
        {
            var client = await this.GetClientAsync();

            client.TileManager.TileButtonPressed += (sender, args) =>
            {
                if (args.TileEvent.TileId == BandConstants.TileId)
                {
                    if (args.TileEvent.ElementId == BandConstants.CheckInElementId)
                    {
                        onCheckIn();
                    }
                    if (args.TileEvent.ElementId == BandConstants.CheckOutElementId)
                    {
                        onCheckOut();
                    }
                }
            };

            await client.TileManager.StartReadingsAsync();

            return client;
        }


        private PageLayout CreatePageLayout()
        {
            var header = new TextBlock
            {
                ElementId = BandConstants.HeaderElementId, ColorSource = ElementColorSource.BandHighlight, AutoWidth = true, Rect = new PageRect(15, 0, 258, 30)
            };
            var checkInButton = new TextButton
            {
                PressedColor = new BandColor(0, 255, 0), ElementId = BandConstants.CheckInElementId, Rect = new PageRect(15, 40, 258, 40),
            };
            var checkOutButton = new TextButton
            {
                PressedColor = new BandColor(255, 0, 0), ElementId = BandConstants.CheckOutElementId, Rect = new PageRect(15, 86, 258, 40),
            };

            var panel = new FilledPanel(header, checkInButton, checkOutButton)
            {
                Rect = new PageRect(0, 0, 258, 128),
            };

            return new PageLayout(panel);
        }

        private PageData CreatePageData()
        {
            return new PageData(BandConstants.TileId, 0, new TextBlockData(BandConstants.HeaderElementId, "c-time"), new TextButtonData(BandConstants.CheckInElementId, "Checkin"), new TextButtonData(BandConstants.CheckOutElementId, "Checkout"));
        }

        private async Task<IBandClient> GetClientAsync()
        {
            var bandInfos = await BandClientManager.Instance.GetBandsAsync();

            if (bandInfos.Any() == false)
                throw new CTimeException();

            return await BandClientManager.Instance.ConnectAsync(bandInfos[0]);
        }

        private async Task<string> FindBandDeviceIdAsync()
        {
            var bandInfo = await BandClientManager.Instance.GetBandsAsync();

            if (bandInfo.Any() == false)
                throw new CTimeException();

            var bandGuid = new Guid("A502CA9A-2BA5-413C-A4E0-13804E47B38F");
            var devices = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(bandGuid)));

            var bandDevice = devices.FirstOrDefault(f => f.Name == bandInfo[0].Name);

            if (bandDevice == null)
                throw new CTimeException();

            return bandDevice.Id;
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Band;

namespace CTime2.Core.Services.Band
{
    public interface IBandService
    {
        Task<bool> IsBandConnectedAsync();

        Task<bool> IsBandTileRegisteredAsync();
        Task RegisterBandTileAsync();
        Task UnRegisterBandTileAsync();

        Task<bool> IsConnectedWithBandAsync();
        Task ConnectWithBandAsync();
        Task DisconnectFromBandAsync();

        Task<IDisposable> ListenForEventsAsync(Action<IBandClient> onCheckIn, Action<IBandClient> onCheckOut);
    }
}
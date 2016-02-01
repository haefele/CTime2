using System;
using System.Threading.Tasks;

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

        Task<IDisposable> ListenForEventsAsync(Action onCheckIn, Action onCheckOut);
    }
}
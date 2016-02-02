using System.Threading.Tasks;

namespace CTime2.Core.Services.Band
{
    public interface IBandService
    {
        Task<bool> IsBandAvailable();

        Task<bool> IsBandTileRegisteredAsync();
        Task RegisterBandTileAsync();
        Task UnRegisterBandTileAsync();

        Task<bool> IsConnectedWithTile();
        Task ConnectWithTileAsync();
        Task DisconnectFromTileAsync();

        Task DisconnectFromBandAsync();
    }
}
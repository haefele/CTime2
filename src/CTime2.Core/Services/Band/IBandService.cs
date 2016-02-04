using System.Threading.Tasks;

namespace CTime2.Core.Services.Band
{
    public class BandInfo
    {
        public string Name { get; set; }
    }

    public interface IBandService
    {
        Task<BandInfo> GetBand();

        Task<bool> IsBandTileRegisteredAsync();
        Task RegisterBandTileAsync();
        Task UnRegisterBandTileAsync();

        Task<bool> IsConnectedWithTile();
        Task ConnectWithTileAsync();
        Task DisconnectFromTileAsync();

        Task DisconnectFromBandAsync();
    }
}
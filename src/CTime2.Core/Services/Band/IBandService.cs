using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace CTime2.Core.Services.Band
{
    public interface IBandService
    {
        Task<BandInfo> GetBand();

        Task<bool> IsBandTileRegisteredAsync();
        Task RegisterBandTileAsync();
        Task UnRegisterBandTileAsync();

        Task HandleTileEventAsync(ValueSet message);
    }
}
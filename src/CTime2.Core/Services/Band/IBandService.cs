using System.Threading.Tasks;
using Windows.Foundation.Collections;

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

        void HandleTileEvent(ValueSet message);
    }
}
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using CTime2.Core.Data;

namespace CTime2.Core.Services.GeoLocation
{
    public interface IGeoLocationService
    {
        Task<GeoLocationState> GetGeoLocationStateAsync(User user);
        Task<Geopoint> TryGetGeoLocationAsync();
    }
}
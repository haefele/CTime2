using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace CTime2.Core.Services.GeoLocation
{
    public interface IGeoLocationService
    {
        Task<bool> HasAccessAsync();
        Task<Geopoint> TryGetGeoLocationAsync();
    }
}
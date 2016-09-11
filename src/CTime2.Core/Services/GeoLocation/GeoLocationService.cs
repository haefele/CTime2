using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace CTime2.Core.Services.GeoLocation
{
    public class GeoLocationService : IGeoLocationService
    {
        public async Task<bool> HasAccessAsync()
        {
            var access = await Geolocator.RequestAccessAsync();

            switch (access)
            {
                case GeolocationAccessStatus.Allowed:
                    return true;

                case GeolocationAccessStatus.Unspecified:
                case GeolocationAccessStatus.Denied:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<Geopoint> TryGetGeoLocationAsync()
        {
            if (await this.HasAccessAsync())
            {
                var location = await new Geolocator().GetGeopositionAsync();
                return location.Coordinate.Point;
            }

            return null;
        }
    }
}
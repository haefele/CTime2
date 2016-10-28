using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using CTime2.Core.Data;

namespace CTime2.Core.Services.GeoLocation
{
    public class GeoLocationService : IGeoLocationService
    {
        public async Task<GeoLocationState> GetGeoLocationStateAsync(User user)
        {
            if (user.SupportsGeoLocation == false)
                return GeoLocationState.NotRequired;

            if (await this.HasAccessAsync() == false)
                return GeoLocationState.RequiredNotAvailable;

            return GeoLocationState.RequiredAndAvailable;
        }

        public async Task<Geopoint> TryGetGeoLocationAsync()
        {
            if (await this.HasAccessAsync())
            {
                var geoLocator = new Geolocator {DesiredAccuracyInMeters = 10};
                var location = await geoLocator.GetGeopositionAsync();

                if (location.Coordinate.Accuracy > 100) //If it's not at least accurate to 100 meters, we don't use the value
                    return null;

                return location.Coordinate.Point;
            }

            return null;
        }

        private async Task<bool> HasAccessAsync()
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
    }
}
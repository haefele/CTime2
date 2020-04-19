using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Caliburn.Micro;
using CTime2.Core.Data;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Analytics;
using UwCore.Common;

namespace CTime2.Core.Services.GeoLocation
{
    public class GeoLocationService : IGeoLocationService
    {
        private static readonly ILog Logger = LogManager.GetLog(typeof(GeoLocationService));

        private readonly IAnalyticsService _analyticsService;

        public GeoLocationService(IAnalyticsService analyticsService)
        {
            Guard.NotNull(analyticsService, nameof(analyticsService));

            this._analyticsService = analyticsService;
        }

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
                try
                {
                    var geoLocator = new Geolocator {DesiredAccuracyInMeters = 10};
                    var location = await geoLocator.GetGeopositionAsync(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10));

                    if (location.Coordinate.Accuracy > 100) //If it's not at least accurate to 100 meters, we don't use the value
                        return null;

                    return location.Coordinate.Point;
                }
                catch (Exception exception)
                {
                    Logger.Warn($"An error occured while getting the current geo-location. {exception.GetFullMessage()}");
                    Logger.Error(exception);

                    this._analyticsService.TrackException(exception);
                }
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
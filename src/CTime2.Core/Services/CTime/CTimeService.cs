using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Caliburn.Micro;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.GeoLocation;
using CTime2.Core.Strings;
using Newtonsoft.Json.Linq;
using UwCore.Common;
using UwCore.Logging;
using UwCore.Services.ApplicationState;

namespace CTime2.Core.Services.CTime
{
    public class CTimeService : ICTimeService
    {
        private static readonly Logger Logger = LoggerFactory.GetLogger<CTimeService>();

        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationStateService _applicationStateService;
        private readonly IGeoLocationService _geoLocationService;

        public CTimeService(IEventAggregator eventAggregator, IApplicationStateService applicationStateService, IGeoLocationService geoLocationService)
        {
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(geoLocationService, nameof(geoLocationService));

            this._eventAggregator = eventAggregator;
            this._applicationStateService = applicationStateService;
            this._geoLocationService = geoLocationService;

            this.AddCTimeCertificate();
        }

        public async Task<User> Login(string emailAddress, string password)
        {
            try
            {
                var responseJson = await this.SendRequestAsync("V2/LoginV2.php", new Dictionary<string, string>
                {
                    {"Password", this.GetHashedPassword(password)},
                    {"LoginName", emailAddress},
                    {"Crypt", 1.ToString()}
                });

                var user = responseJson?
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .FirstOrDefault();

                if (user == null)
                    return null;

                return new User
                {
                    Id = user.Value<string>("EmployeeGUID"),
                    CompanyId = user.Value<string>("CompanyGUID"),
                    Email = user.Value<string>("LoginName"),
                    FirstName = user.Value<string>("EmployeeFirstName"),
                    Name = user.Value<string>("EmployeeName"),
                    ImageAsPng = Convert.FromBase64String(user.Value<string>("EmployeePhoto") ?? string.Empty),
                    SupportsGeoLocation = user.Value<int>("GeolocationAllowed") == 1,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.Login)}. Email address: {emailAddress}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLogin") , exception);
            }
        }

        public async Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end)
        {
            try
            {
                var responseJson = await this.SendRequestAsync("V2/GetTimerListV2.php", new Dictionary<string, string>
                {
                    {"EmployeeGUID", employeeGuid},
                    {"DateTill", end.ToString("yyyy-MM-dd")},
                    {"DateFrom", start.ToString("yyyy-MM-dd")},
                    {"Summary", 1.ToString()}
                });

                if (responseJson == null)
                    return new List<Time>();
                
                return (responseJson
                    .Value<JArray>("Result") ?? new JArray())
                    .OfType<JObject>()
                    .Select(f => new Time
                    {
                        Day = f.Value<DateTime>("DayDate"),
                        Hours = TimeSpan.Parse(f.Value<string>("TimeHour_IST_HR")),
                        State = f["TimeTrackTypePure"].ToObject<int?>() != 0 
                            ? (TimeState?)f["TimeTrackTypePure"].ToObject<int?>()
                            : null,
                        ClockInTime = f["TimeTrackIn"].ToObject<DateTime?>(),
                        ClockOutTime = f["TimeTrackOut"].ToObject<DateTime?>(),
                    })
                    .Select(f =>
                    {
                        if (f.ClockInTime != null && f.ClockOutTime != null)
                        {
                            f.State = (f.State ?? 0) | TimeState.Left;
                        }
                        else if (f.ClockInTime != null)
                        {
                            f.State = (f.State ?? 0) | TimeState.Entered;
                        }

                        return f;
                    })
                    .Where(f => f.Day <= DateTime.Today || f.ClockInTime != null || f.ClockOutTime != null)
                    .ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.GetTimes)}. Employee: {employeeGuid}, Start: {start}, End: {end}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLoadingTimes"), exception);
            }
        }

        public async Task SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state, bool withGeolocation)
        {
            try
            {
                Geopoint location = withGeolocation
                    ? await this._geoLocationService.TryGetGeoLocationAsync()
                    : null;

                var responseJson = await this.SendRequestAsync("V2/SaveTimerV2.php", new Dictionary<string, string>
                {
                    {"TimerKind", ((int) state).ToString()},
                    {"TimerText", string.Empty},
                    {"TimerTime", time.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"EmployeeGUID", employeeGuid},
                    {"GUID", companyId},
                    {"lat", location?.Position.Latitude.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                    {"long", location?.Position.Longitude.ToString(CultureInfo.InvariantCulture) ?? string.Empty }
                });

                if (responseJson?.Value<int>("State") == 0)
                {
                    this._eventAggregator.PublishOnCurrentThread(new UserStamped());
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.SaveTimer)}. Employee: {employeeGuid}, Time: {time}, Company Id: {companyId}, State: {(int)state}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileStamp"), exception);
            }
        }

        public async Task<Time> GetCurrentTime(string employeeGuid)
        {
            try
            {
                IList<Time> timesForToday = await this.GetTimes(employeeGuid, DateTime.Today, DateTime.Today.AddDays(1));
            
                var itemsToIgnore = timesForToday
                    .Where(f =>
                        (f.ClockInTime != null && f.ClockOutTime != null) ||
                        (f.ClockInTime == null && f.ClockOutTime == null))
                    .ToList();

                Time latestFinishedTimeToday = itemsToIgnore
                    .Where(f => f.ClockInTime != null && f.ClockOutTime != null)
                    .OrderByDescending(f => f.ClockOutTime)
                    .FirstOrDefault();

                return timesForToday.FirstOrDefault(f => itemsToIgnore.Contains(f) == false) ?? latestFinishedTimeToday;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.GetCurrentTime)}. Employee: {employeeGuid}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLoadingCurrentTime"), exception);
            }
        }

        public async Task<IList<AttendingUser>> GetAttendingUsers(string companyId)
        {
            try
            {
                var cacheEtag = this._applicationStateService.GetAttendanceListImageCacheEtag();

                var responseJson = await this.SendRequestAsync("V2/GetPresenceListV2.php", new Dictionary<string, string>
                {
                    {"GUID", companyId},
                    {"cacheDate", cacheEtag ?? string.Empty }
                });

                if (responseJson == null)
                    return new List<AttendingUser>();

                var newCacheEtag = responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .Select(f => f.Value<string>("cacheDate"))
                    .FirstOrDefault();

                this._applicationStateService.SetAttendanceListImageCacheEtag(newCacheEtag);

                var result = responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .Select(f => new
                    {
                        EmployeeI3D = f.Value<int>("EmployeeI3D"),
                        Employee = new AttendingUser
                        {
                            Name = f.Value<string>("EmployeeName"),
                            FirstName = f.Value<string>("EmployeeFirstName"),
                            IsAttending = f.Value<int>("PresenceStatus") == 1,
                            ImageAsPng = Convert.FromBase64String(f.Value<string>("EmployeePhoto") ?? string.Empty),
                        }
                    })
                    .ToDictionary(f => f.EmployeeI3D, f => f.Employee);

                var imageCache = new EmployeeImageCache();
                if (newCacheEtag == cacheEtag)
                {
                    await imageCache.FillWithCachedImages(result);
                }

                if (newCacheEtag != cacheEtag)
                {
                    await imageCache.CacheImagesAsync(result);
                }

                return result.Select(f => f.Value).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.GetAttendingUsers)}. Company Id: {companyId}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLoadingAttendanceList"), exception);
            }
        }
        
        private string GetHashedPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashedPasswordBytes = MD5.Create().ComputeHash(passwordBytes);
            var hashedPasswordString = BitConverter.ToString(hashedPasswordBytes);

            return hashedPasswordString.Replace("-", string.Empty).ToLower();
        }

        private async Task<JObject> SendRequestAsync(string function, Dictionary<string, string> data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri(function))
            {
                Content = new HttpFormUrlEncodedContent(data)
            };

            var response = await this.GetClient().SendRequestAsync(request);

            if (response.StatusCode != HttpStatusCode.Ok)
                return null;

            var responseContentAsString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseContentAsString);

            var responseState = responseJson.Value<int>("State");

            if (responseState != 0)
                return null;

            return responseJson;
        }

        private Uri BuildUri(string path)
        {
            return new Uri($"https://app.c-time.net/php/{path}");
        }

        private HttpClient GetClient()
        {
            var filter = new HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);

            return new HttpClient(filter);
        }

        private void AddCTimeCertificate()
        {
            using (var certificateStream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("CTime2.Core.Services.CTime.Certificate.cer"))
            using (var memoryStream = new MemoryStream())
            {
                certificateStream.CopyTo(memoryStream);
                var buffer = memoryStream.ToArray().AsBuffer();

                var certificate = new Certificate(buffer);
                CertificateStores.TrustedRootCertificationAuthorities.Add(certificate);
            }
        }

        #region Internal
        private class EmployeeImageCache
        {
            public async Task FillWithCachedImages(Dictionary<int, AttendingUser> users)
            {
                foreach (var user in users)
                {
                    user.Value.ImageAsPng = await this.GetCachedImageAsync(user.Key);
                }
            }

            private async Task<byte[]> GetCachedImageAsync(int employeeI3D)
            {
                var imageFileName = this.GetImageName(employeeI3D);
                var imagesFolder = await this.GetImagesFolderAsync();

                if (await imagesFolder.TryGetItemAsync(imageFileName) == null)
                    return null;

                var imageFile = await imagesFolder.GetFileAsync(imageFileName);

                using (var imageStream = await imageFile.OpenStreamForReadAsync())
                using (var memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);

                    return memoryStream.ToArray();
                }
            }

            public async Task CacheImagesAsync(Dictionary<int, AttendingUser> users)
            {
                foreach (var user in users)
                {
                    await this.CacheImageAsync(user.Key, user.Value.ImageAsPng);
                }
            }

            private async Task CacheImageAsync(int employeeI3D, byte[] image)
            {
                var imageFileName = this.GetImageName(employeeI3D);
                var imagesFolder = await this.GetImagesFolderAsync();
                
                var imageFile = await imagesFolder.CreateFileAsync(imageFileName, CreationCollisionOption.ReplaceExisting);
                using (var imageStream = await imageFile.OpenStreamForWriteAsync())
                {
                    await imageStream.WriteAsync(image, 0, image.Length);
                }
            }

            private string GetImageName(int employeeI3D)
            {
                return $"AttendingUser-{employeeI3D}.png";
            }

            private async Task<StorageFolder> GetImagesFolderAsync()
            {
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync("AttendingUserImages", CreationCollisionOption.OpenIfExists);
            }
        }
        #endregion
    }
}
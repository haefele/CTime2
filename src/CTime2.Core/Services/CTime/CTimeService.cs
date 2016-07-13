using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Strings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UwCore.Logging;

namespace CTime2.Core.Services.CTime
{
    public class CTimeService : ICTimeService
    {
        private static readonly Logger Logger = LoggerFactory.GetLogger<CTimeService>();

        public CTimeService()
        {
            this.AddCTimeCertificate();
        }

        public async Task<User> Login(string emailAddress, string password)
        {
            try
            {
                var responseJson = await this.SendRequestAsync("GetRFIDbyLoginName.php", new Dictionary<string, string>
                {
                    {"Password", password},
                    {"LoginName", emailAddress}
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
                var responseJson = await this.SendRequestAsync("GetTimeTrackList.php", new Dictionary<string, string>
                {
                    {"EmployeeGUID", employeeGuid},
                    {"DateTill", end.ToString("dd.MM.yyyy")},
                    {"DateFrom", start.ToString("dd.MM.yyyy")}
                });

                if (responseJson == null)
                    return new List<Time>();
                
                return responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .Select(f => new Time
                    {
                        Day = f.Value<DateTime>("day"),
                        Hours = TimeSpan.FromHours(double.Parse(f.Value<string>("DayHours") ?? "0", CultureInfo.InvariantCulture)),
                        State = f["TimeTrackType"].ToObject<int?>() == 0 ? null : (TimeState?)f["TimeTrackType"].ToObject<int?>(),
                        ClockInTime = f["TimeTrackIn"].ToObject<DateTime?>(),
                        ClockOutTime = f["TimeTrackOut"].ToObject<DateTime?>(),
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.GetTimes)}. Employee: {employeeGuid}, Start: {start}, End: {end}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLoadingTimes"), exception);
            }
        }

        public async Task<bool> SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state)
        {
            try
            {
                var responseJson = await this.SendRequestAsync("SaveTimer.php", new Dictionary<string, string>
                {
                    {"RFID", string.Empty},
                    {"TimerKind", ((int) state).ToString()},
                    {"TimerText", string.Empty},
                    {"TimerTime", time.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"EmployeeGUID", employeeGuid},
                    {"GUID", companyId}
                });

                return responseJson?.Value<int>("State") == 0;
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
                var responseJson = await this.SendRequestAsync("GetPresenceList.php", new Dictionary<string, string>
                {
                    {"GUID", companyId}
                });

                if (responseJson == null)
                    return new List<AttendingUser>();

                return responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .Select(f => new AttendingUser
                    {
                        Name = f.Value<string>("EmployeeName"),
                        FirstName = f.Value<string>("EmployeeFirstName"),
                        IsAttending = f.Value<int>("PresenceStatus") == 1,
                        ImageAsPng = Convert.FromBase64String(f.Value<string>("EmployeePhoto") ?? string.Empty),
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception in method {nameof(this.GetAttendingUsers)}. Company Id: {companyId}");
                throw new CTimeException(CTime2CoreResources.Get("CTimeService.ErrorWhileLoadingAttendanceList"), exception);
            }
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
    }
}
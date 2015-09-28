using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;
using CTime2.Core.Common;
using CTime2.Core.Data;
using Newtonsoft.Json.Linq;

namespace CTime2.Core.Services.CTime
{
    public class CTimeService : ICTimeService
    {
        public async Task<User> Login(string companyId, string emailAddress, string password)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri("/ctimetrack/php/GetRFIDbyLoginName.php"))
                {
                    Content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "Password", password },
                        { "LoginName", emailAddress},
                        { "GUIDlogin", companyId },
                    })
                };

                var response = await this.GetClient().SendRequestAsync(request);

                if (response.StatusCode != HttpStatusCode.Ok)
                    return null;

                var responseContentAsString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContentAsString);

                var state = responseJson.Value<int>("State");

                if (state != 0)
                    return null;

                var user = responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .FirstOrDefault();

                if (user == null)
                    return null;

                return new User
                {
                    Id = user.Value<string>("EmployeeGUID"),
                    Email = user.Value<string>("LoginName"),
                    FirstName = user.Value<string>("EmployeeFirstName"),
                    Name = user.Value<string>("EmployeeName"),
                    ImageAsPng = Convert.FromBase64String(user.Value<string>("EmployeePhoto") ?? string.Empty),
                };
            }
            catch (Exception exception)
            {
                throw new CTimeException("Beim Anmelden ist ein Fehler aufgetreten.", exception);
            }
        }

        public async Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri("/ctimetrack/php/GetTimeTrackList.php"))
                {
                    Content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "EmployeeGUID", employeeGuid },
                        { "DateTill", end.ToString("dd.MM.yyyy") },
                        { "DateFrom", start.ToString("dd.MM.yyyy") }
                    })
                };

                var response = await this.GetClient().SendRequestAsync(request);

                if (response.StatusCode != HttpStatusCode.Ok)
                    return new List<Time>();

                var responseContentAsString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContentAsString);

                var state = responseJson.Value<int>("State");

                if (state != 0)
                    return new List<Time>();

                return responseJson
                    .Value<JArray>("Result")
                    .OfType<JObject>()
                    .Select(f => new Time
                    {
                        Day = f.Value<DateTime>("day"),
                        Hours = TimeSpan.FromHours(double.Parse((f.Value<string>("DayHours") ?? "0").Replace(".", ","))),
                        State = (TimeState?)f.Value<int?>("TimeTrackType"),
                        ClockInTime = f.Value<DateTime?>("TimeTrackIn"),
                        ClockOutTime = f.Value<DateTime?>("TimeTrackOut"),
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                throw new CTimeException("Beim Laden der Zeiten ist ein Fehler aufgetreten.", exception);
            }
        }

        public async Task<bool> SaveTimer(string employeeGuid, DateTime time, string companyId, TimeState state)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri("/ctimetrack/php/SaveTimer.php"))
                {
                    Content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "RFID", string.Empty },
                        { "TimerKind", ((int)state).ToString() },
                        { "TimerText", string.Empty },
                        { "TimerTime", time.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "EmployeeGUID", employeeGuid },
                        { "GUID", companyId }
                    })
                };

                var response = await this.GetClient().SendRequestAsync(request);

                if (response.StatusCode != HttpStatusCode.Ok)
                    return false;

                var responseContentAsString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContentAsString);

                var responseState = responseJson.Value<int>("State");
                return responseState == 0;
            }
            catch (Exception exception)
            {
                throw new CTimeException("Beim Stempeln ist ein Fehler aufgetreten.", exception);
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
                throw new CTimeException("Beim Laden der aktuellen Zeit ist ein Fehler aufgetreten.", exception);
            }
        }

        public async Task<IList<AttendingUser>> GetAttendingUsers(string companyId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri("/ctimetrack/php/GetPresenceList.php"))
                {
                    Content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "GUID", companyId }
                    })
                };

                var response = await this.GetClient().SendRequestAsync(request);

                if (response.StatusCode != HttpStatusCode.Ok)
                    return new List<AttendingUser>();

                var responseContentAsString = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContentAsString);

                var responseState = responseJson.Value<int>("State");

                if (responseState != 0)
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
                throw new CTimeException("Beim Laden der Anwesenheitsliste ist ein Fehler aufgetreten.", exception);
            }
        }

        private Uri BuildUri(string path)
        {
            return new Uri($"http://c-time.cloudapp.net{path}");
        }

        private HttpClient GetClient()
        {
            return new HttpClient();
        }
    }
}
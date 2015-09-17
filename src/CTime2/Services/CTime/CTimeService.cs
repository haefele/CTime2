using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;

namespace CTime2.Services.CTime
{
    public class CTimeService : ICTimeService
    {
        public async Task<User> Login(string companyId, string emailAddress, string password)
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
            var responseJson = JsonObject.Parse(responseContentAsString);

            var state = (int)responseJson.GetNamedNumber("State");

            if (state != 0)
                return null;

            var user = responseJson.GetNamedArray("Result").GetObjectAt(0);

            return new User
            {
                Id = user.GetNamedString("EmployeeGUID"),
                Email = user.GetNamedString("LoginName"),
                FirstName = user.GetNamedString("EmployeeFirstName"),
                Name = user.GetNamedString("EmployeeName")
            };
        }

        public async Task<IList<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end)
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
                return null;

            return responseJson
                .Value<JArray>("Result")
                .OfType<JObject>()
                .Select(f => new Time
                {
                    Day = f.Value<DateTime>("day"),
                    Hours = TimeSpan.FromHours(double.Parse(f.Value<string>("DayHours") ?? "0")),
                    State = (TimeState?)f.Value<int?>("TimeTrackType"),
                })
                .ToList();
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
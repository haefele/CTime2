using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

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
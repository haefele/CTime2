using System.Threading.Tasks;
using Windows.Storage;
using CTime2.Services.CTime;
using Newtonsoft.Json;

namespace CTime2.Services.SessionState
{
    public class SessionStateService : ISessionStateService
    {
        public User CurrentUser { get; set; }
        public Task SaveStateAsync()
        {
            var container = this.GetStateContainer();

            container.Values[nameof(this.CurrentUser)] = JsonConvert.SerializeObject(this.CurrentUser);

            return Task.CompletedTask;
        }

        public Task RestoreStateAsync()
        {
            var container = this.GetStateContainer();

            if (container.Values.ContainsKey(nameof(this.CurrentUser)))
                this.CurrentUser = JsonConvert.DeserializeObject<User>((string)container.Values[nameof(this.CurrentUser)]);

            return Task.CompletedTask;
        }

        private ApplicationDataContainer GetStateContainer()
        {
            return ApplicationData.Current.LocalSettings.CreateContainer("CTime2.SessionState", ApplicationDataCreateDisposition.Always);
        }
    }
}
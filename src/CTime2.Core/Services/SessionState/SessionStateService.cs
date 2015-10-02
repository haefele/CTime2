using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using CTime2.Core.Common;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;
using Newtonsoft.Json;

namespace CTime2.Core.Services.SessionState
{
    public class SessionStateService : ISessionStateService
    {
        public string CompanyId { get; set; }
        public User CurrentUser { get; set; }

        public async Task SaveStateAsync()
        {
            try
            {
                var settings = new Dictionary<string, object>
                {
                    [nameof(this.CurrentUser)] = this.CurrentUser,
                    [nameof(this.CompanyId)] = this.CompanyId
                };

                await this.WriteSettingsAsync(settings);
            }
            catch (Exception exception)
            {
                throw new CTimeException("Beim Speichern des Logins ist ein Fehler aufgetreten.", exception);
            }
        }

        public async Task RestoreStateAsync()
        {
            try
            {
                var settings = await this.ReadSettingsAsync();

                if (settings.ContainsKey(nameof(this.CompanyId)))
                    this.CompanyId = (string)settings[nameof(this.CompanyId)];
            
                if (settings.ContainsKey(nameof(this.CurrentUser)))
                    this.CurrentUser = (User)settings[nameof(this.CurrentUser)];
            }
            catch (Exception exception)
            {
                throw new CTimeException("Beim Laden des Logins ist ein Fehler aufgetreten.", exception);
            }
        }

        #region Private Methods
        private async Task<Dictionary<string, object>> ReadSettingsAsync()
        {
            var settingsFile = await this.GetSettingsFileAsync(CreationCollisionOption.OpenIfExists);

            using (var stream = await settingsFile.OpenStreamForReadAsync())
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                var serializer = this.GetJsonSerializer();
                return serializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(streamReader)) ?? new Dictionary<string, object>();
            }
        }

        private async Task WriteSettingsAsync(Dictionary<string, object> settings)
        {
            var settingsFile = await this.GetSettingsFileAsync(CreationCollisionOption.ReplaceExisting);

            using (var stream = await settingsFile.OpenStreamForWriteAsync())
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                var serializer = this.GetJsonSerializer();
                serializer.Serialize(streamWriter, settings);
            }
        }

        private async Task<StorageFile> GetSettingsFileAsync(CreationCollisionOption option)
        {
            return await ApplicationData.Current.LocalFolder.CreateFileAsync("Settings.bin", option);
        }

        private JsonSerializer GetJsonSerializer()
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }
        #endregion
    }
}
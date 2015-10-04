using Windows.ApplicationModel.Resources;

namespace CTime2.Core.Resources
{
    public class ResourceAccessor
    {
        private readonly ResourceLoader _resourceLoader;

        public ResourceAccessor(ResourceLoader resourceLoader)
        {
            this._resourceLoader = resourceLoader;
        }
        
        public string Get(string resource)
        {
            resource = resource.Replace(".", "/");

            return this._resourceLoader.GetString(resource);
        }

        public string GetFormatted(string resource, params object[] arguments)
        {
            return string.Format(this.Get(resource), arguments);
        }
    }
}
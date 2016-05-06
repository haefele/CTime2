using Windows.ApplicationModel.Resources;
using UwCore.Common;

namespace CTime2.Strings
{
    public static class CTime2Resources
    {
        private static readonly ResourceAccessor _accessor = new ResourceAccessor(ResourceLoader.GetForCurrentView("Resources"));
        
        public static string Get(string resource)
        {
            return _accessor.Get(resource);
        }

        public static string GetFormatted(string resource, params object[] arguments)
        {
            return _accessor.GetFormatted(resource, arguments);
        }
    }
}
using Windows.ApplicationModel.Resources;
using CTime2.Core.Resources;

namespace CTime2.VoiceCommandService.Strings
{
    public static class CTime2VoiceCommandServiceResources
    {
        private static readonly ResourceAccessor _accessor = new ResourceAccessor(ResourceLoader.GetForViewIndependentUse("CTime2.VoiceCommandService/Resources"));

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
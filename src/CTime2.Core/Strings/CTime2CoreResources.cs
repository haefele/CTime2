using Windows.ApplicationModel.Resources;
using CTime2.Core.Resources;

namespace CTime2.Core.Strings
{
    public static class CTime2CoreResources
    {
        private static readonly ResourceAccessor _accessor;

        static CTime2CoreResources()
        {
            _accessor = new ResourceAccessor("CTime2.Core/Resources");
        }

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
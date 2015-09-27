using System;
using Windows.ApplicationModel;

namespace CTime2.Extensions
{
    public static class PackageVersionExtensions
    {
        public static Version ToVersion(this PackageVersion self)
        {
            return new Version(self.Major, self.Minor, self.Build, self.Revision);
        }
    }
}
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using UwCore.Common;

namespace CTime2.Core.Services.Sharing
{
    public static class SharingServiceExtensions
    {
        public static void Share(this ISharingService self, string title, Action<DataPackage> customizeDataPackage)
        {
            Guard.NotNull(self, nameof(self));
            Guard.NotNullOrWhiteSpace(title, nameof(title));
            Guard.NotNull(customizeDataPackage, nameof(customizeDataPackage));

            self.Share(title, package =>
            {
                customizeDataPackage(package);
                return Task.CompletedTask;
            });
        }
    }
}
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using UwCore.Common;

namespace CTime2.Core.Services.Sharing
{
    public class SharingService : ISharingService
    {
        public void Share(string title, Func<DataPackage, Task> customizeDataPackage)
        {
            Guard.NotNullOrWhiteSpace(title, nameof(title));
            Guard.NotNull(customizeDataPackage, nameof(customizeDataPackage));

            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += async (sender, args) =>
            {
                args.Request.Data.Properties.Title = title;

                var deferral = args.Request.GetDeferral();
                await customizeDataPackage(args.Request.Data);
                deferral.Complete();
            };
            
            DataTransferManager.ShowShareUI();
        }
    }
}
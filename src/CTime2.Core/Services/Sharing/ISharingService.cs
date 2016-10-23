using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace CTime2.Core.Services.Sharing
{
    public interface ISharingService
    {
        void Share(string title, Func<DataPackage, Task> customizeDataPackage);
    }
}
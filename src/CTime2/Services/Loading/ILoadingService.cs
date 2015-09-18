using System;

namespace CTime2.Services.Loading
{
    public interface ILoadingService
    {
        IDisposable Show(string message);
    }
}
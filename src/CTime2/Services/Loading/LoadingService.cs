using System;
using CTime2.Controls;
using CTime2.Core.Common;

namespace CTime2.Services.Loading
{
    public class LoadingService : ILoadingService
    {
        private readonly LoadingOverlay _overlay;

        public LoadingService(LoadingOverlay overlay)
        {
            this._overlay = overlay;
        }

        public IDisposable Show(string message)
        {
            if (this._overlay.IsActive)
                return new DisposableAction(() => { });

            this._overlay.Message = message;
            this._overlay.IsActive = true;

            return new DisposableAction(() =>
            {
                this._overlay.IsActive = false;
            });
        }
    }
}
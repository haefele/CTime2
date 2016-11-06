using System.Reactive;
using System.Threading.Tasks;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.Dialog;

namespace CTime2.Views.HeaderDetails
{
    public class HeaderDetailsViewModel : UwCoreScreen
    {
        private readonly IDialogService _dialogService;

        public ReactiveCommand<Unit> ShowNoInternetConnection { get; }

        public HeaderDetailsViewModel(IDialogService dialogService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));
            
            this._dialogService = dialogService;

            this.ShowNoInternetConnection = ReactiveCommand.CreateAsyncTask(_ => this.ShowNoInternetConnectionImpl());
        }

        private Task ShowNoInternetConnectionImpl()
        {
            var message = CTime2Resources.Get("NoInternetConnection.Message");
            var title = CTime2Resources.Get("NoInternetConnection.Title");

            return this._dialogService.ShowAsync(message, title);
        }
    }
}
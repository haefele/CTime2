using System;
using System.Threading.Tasks;
using CTime2.Core.Common;
using CTime2.Extensions;
using CTime2.Services.Dialog;

namespace CTime2.Services.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IDialogService _dialogService;

        public ExceptionHandler(IDialogService dialogService)
        {
            this._dialogService = dialogService;
        }


        public async Task HandleAsync(Exception exception)
        {
            if (exception is CTimeException)
            {
                await this._dialogService.ShowAsync(exception.Message, "Fehler");
            }
            else
            {
                //TODO: Logging
                await this._dialogService.ShowAsync("Es ist ein Fehler aufgetreten.", "Fehler");
            }
        }
    }
}
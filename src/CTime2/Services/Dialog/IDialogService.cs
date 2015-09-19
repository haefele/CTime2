using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace CTime2.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAsync(string message, string title, IEnumerable<UICommand> commands);
    }
}
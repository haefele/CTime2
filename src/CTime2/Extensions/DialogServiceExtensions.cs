using System.Threading.Tasks;
using CTime2.Services.Dialog;

namespace CTime2.Extensions
{
    public static class DialogServiceExtensions
    {
        public static Task ShowAsync(this IDialogService self, string message)
        {
            return self.ShowAsync(message, null, null);
        }

        public static Task ShowAsync(this IDialogService self, string message, string title)
        {
            return self.ShowAsync(message, title, null);
        }
    }
}
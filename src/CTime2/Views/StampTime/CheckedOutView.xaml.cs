using Windows.UI.Xaml.Controls;

namespace CTime2.Views.StampTime
{
    public sealed partial class CheckedOutView
    {
        public CheckedOutViewModel TimeStateViewModel => this.DataContext as CheckedOutViewModel;

        public CheckedOutView()
        {
            this.InitializeComponent();
        }
    }
}

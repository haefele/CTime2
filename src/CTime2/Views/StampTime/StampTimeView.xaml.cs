using Windows.UI.Xaml.Controls;

namespace CTime2.Views.StampTime
{
    public sealed partial class StampTimeView : Page
    {
        public StampTimeView()
        {
            this.InitializeComponent();
        }

        public StampTimeViewModel ViewModel => this.DataContext as StampTimeViewModel;
    }
}

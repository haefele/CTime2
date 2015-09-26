using Windows.UI.Xaml.Controls;

namespace CTime2.Views.YourTimes
{
    public sealed partial class YourTimesView : Page
    {
        public YourTimesViewModel ViewModel => this.DataContext as YourTimesViewModel;

        public YourTimesView()
        {
            this.InitializeComponent();
        }
    }
}

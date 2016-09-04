using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics
{
    public sealed partial class DetailedStatisticView : Page
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public DetailedStatisticView()
        {
            this.InitializeComponent();
        }
    }
}

using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.Details.OverTime
{
    public sealed partial class OverTimeView : Page
    {
        public OverTimeViewModel ViewModel => this.DataContext as OverTimeViewModel;

        public OverTimeView()
        {
            this.InitializeComponent();
        }
    }
}

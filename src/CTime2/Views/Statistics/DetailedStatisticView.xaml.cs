using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace CTime2.Views.Statistics
{
    public sealed partial class DetailedStatisticView : Page
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public DetailedStatisticView()
        {
            this.InitializeComponent();

            this.ValueAxis.Interval = 1;
            this.ValueAxis.Minimum = 0;
            
            this.DateTimeAxis.IntervalType = DateTimeIntervalType.Weeks;
        }
    }
}

using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using WinRTXamlToolkit.Controls.Extensions;

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

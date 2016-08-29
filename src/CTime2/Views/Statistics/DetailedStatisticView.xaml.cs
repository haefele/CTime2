using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Caliburn.Micro;
using CTime2.Views.YourTimes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using WinRTXamlToolkit.Controls.Extensions;
using INavigationService = UwCore.Services.Navigation.INavigationService;

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

        private void DataPointSeries_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedChartItem = (StatisticChartItem) e.AddedItems.First();

            var navigationService = IoC.Get<INavigationService>();
            navigationService
                .For<YourTimesViewModel>()
                .WithParam(f => f.StartDate, new DateTimeOffset(selectedChartItem.Date))
                .WithParam(f => f.EndDate, new DateTimeOffset(selectedChartItem.Date))
                .Navigate();
        }
    }
}

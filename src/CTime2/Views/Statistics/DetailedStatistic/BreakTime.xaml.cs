using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.DetailedStatistic
{
    public sealed partial class BreakTime : Page, ICustomDataPointFormat
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public BreakTime()
        {
            this.InitializeComponent();

            this.ValueAxis.Minimum = 0;
        }

        private void DataPointSeries_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.NavigateTo((StatisticChartItem)e.AddedItems.First());
        }

        string ICustomDataPointFormat.Format(double value)
        {
            return Math.Round(value).ToString();
        }
    }
}

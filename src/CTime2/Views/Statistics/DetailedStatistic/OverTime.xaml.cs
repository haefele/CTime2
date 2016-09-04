using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using CTime2.Strings;

namespace CTime2.Views.Statistics.DetailedStatistic
{
    public sealed partial class OverTime : Page, ICustomDataPointFormat
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public OverTime()
        {
            this.InitializeComponent();
        }

        private void DataPointSeries_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.GoToMyTimesForStatisticChartItem((StatisticChartItem)e.AddedItems.First());
        }

        string ICustomDataPointFormat.Format(double value)
        {
            var overTime = (int)Math.Round(value);
            return CTime2Resources.GetFormatted("Statistics.OverTimeDataPointFormat", overTime);
        }
    }
}

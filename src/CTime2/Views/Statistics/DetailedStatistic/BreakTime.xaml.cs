using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using CTime2.Strings;

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
            this.ViewModel.GoToMyTimesForStatisticChartItem((StatisticChartItem)e.AddedItems.First());
        }

        string ICustomDataPointFormat.Format(double value)
        {
            var breakTime = (int)Math.Round(value);
            return CTime2Resources.GetFormatted("Statistics.BreakTimeDataPointFormat", breakTime);
        }
    }
}

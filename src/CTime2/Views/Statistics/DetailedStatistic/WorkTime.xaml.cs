using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.DetailedStatistic
{
    public sealed partial class WorkTime : Page, ICustomDataPointFormat
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public WorkTime()
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
            var timeSpan = TimeSpan.FromHours(value);
            return string.Format("{0} h {1} min", timeSpan.Hours, timeSpan.Minutes);
        }
    }
}

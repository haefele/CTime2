using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Uwp;
using ReactiveUI;

namespace CTime2.Views.Statistics
{
    public class StatisticChartItemsToSeriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var chartItems = value as ReactiveList<StatisticChartItem>[];

            if (chartItems == null)
                return null;

            var dayConfig = Mappers.Xy<StatisticChartItem>()
                .X(dayModel => (double)dayModel.Date.Ticks / TimeSpan.FromDays(1).Ticks)
                .Y(dayModel => dayModel.Value);

            var seriesCollection = new SeriesCollection(dayConfig);

            foreach (var line in chartItems)
            {
                seriesCollection.Add(new LineSeries
                {
                    Values = new ChartValues<StatisticChartItem>(line),
                    LineSmoothness = 0,
                    DataLabels = true
                });   
            }

            return seriesCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
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

        private bool _addedLines = false;
        private void LineChart_OnLayoutUpdated(object sender, object e)
        {
            var canvas = this.LineChart.GetDescendantsOfType<Canvas>().FirstOrDefault(f => f.Name == "CustomLineCanvas");
            var plotArea = this.LineChart.GetDescendantsOfType<Grid>().FirstOrDefault(f => f.Name == "PlotArea");

            if (canvas == null || plotArea == null)
                return;

            if (this.LineSeries.ItemsSource == null)
                return;

            if (this._addedLines)
                return;

            int index = 0;
            foreach (StatisticChartItem item in this.LineSeries.ItemsSource)
            {
                if (item.Date.DayOfWeek == DayOfWeek.Saturday ||
                    item.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    var point = this.LineSeries.Points[index];

                    var styleName = item.Date.DayOfWeek == DayOfWeek.Saturday
                        ? "SaturdayLineStyle"
                        : "SundayLineStyle";

                    var line = new Line
                    {
                        Style = (Style) this.Resources[styleName],
                        X1 = point.X,
                        X2 = point.X,
                        Y1 = -10.0,
                        Y2 = plotArea.ActualHeight
                    };

                    canvas.Children.Add(line);
                }

                index++;
            }

            this._addedLines = true;
        }
    }
}

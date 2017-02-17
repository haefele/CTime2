using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Telerik.UI.Xaml.Controls.Chart;

namespace CTime2.Views.Statistics.Details.WorkTime
{
    public sealed partial class WorkTimeView : Page
    {
        public WorkTimeViewModel ViewModel => this.DataContext as WorkTimeViewModel;

        public WorkTimeView()
        {
            this.InitializeComponent();
            
            var annotation = new CartesianGridLineAnnotation
            {
                Axis = this.Chart.HorizontalAxis,
                Value = new DateTime(2017, 2, 5, 12, 0, 0),
                Stroke = new SolidColorBrush(Colors.DimGray),
                Foreground = new SolidColorBrush(Colors.DimGray),
                StrokeThickness = 1,
                Label = "KW 6",
                LabelDefinition = new ChartAnnotationLabelDefinition
                {
                    Location = ChartAnnotationLabelLocation.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalOffset = 4,
                }
            };
            this.Chart.Annotations.Add(annotation);

            var annotation2 = new CartesianGridLineAnnotation
            {
                Axis = this.Chart.HorizontalAxis,
                Value = new DateTime(2017, 2, 12, 12, 0, 0),
                Stroke = new SolidColorBrush(Colors.DimGray),
                Foreground = new SolidColorBrush(Colors.DimGray),
                StrokeThickness = 1,
                Label = "KW 7",
                LabelDefinition = new ChartAnnotationLabelDefinition
                {
                    Location = ChartAnnotationLabelLocation.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalOffset = 4,
                }
            };
            this.Chart.Annotations.Add(annotation2);

            var saturdayColor = (Color)this.Resources["SystemAccentColor"];
            saturdayColor.A = 40;
            var sundayColor = (Color)this.Resources["SystemAccentColor"];
            sundayColor.A = 80;

            var annotation3 = new CartesianMarkedZoneAnnotation
            {
                VerticalFrom = 0,
                VerticalTo = 1000000,
                HorizontalFrom = new DateTime(2017, 2, 3, 12, 0, 0),
                HorizontalTo = new DateTime(2017, 2, 4, 12, 0, 0),
                Fill = new SolidColorBrush(saturdayColor)
            };
            this.Chart.Annotations.Add(annotation3);
            var annotation4 = new CartesianMarkedZoneAnnotation
            {
                VerticalFrom = 0,
                VerticalTo = 1000000,
                HorizontalFrom = new DateTime(2017, 2, 4, 12, 0, 0),
                HorizontalTo = new DateTime(2017, 2, 5, 12, 0, 0),
                Fill = new SolidColorBrush(sundayColor)
            };
            this.Chart.Annotations.Add(annotation4);

            var annotation5 = new CartesianMarkedZoneAnnotation
            {
                VerticalFrom = 0,
                VerticalTo = 1000000,
                HorizontalFrom = new DateTime(2017, 2, 10, 12, 0, 0),
                HorizontalTo = new DateTime(2017, 2, 11, 12, 0, 0),
                Fill = new SolidColorBrush(saturdayColor)
            };
            this.Chart.Annotations.Add(annotation5);
            var annotation6 = new CartesianMarkedZoneAnnotation
            {
                VerticalFrom = 0,
                VerticalTo = 1000000,
                HorizontalFrom = new DateTime(2017, 2, 11, 12, 0, 0),
                HorizontalTo = new DateTime(2017, 2, 12, 12, 0, 0),
                Fill = new SolidColorBrush(sundayColor)
            };
            this.Chart.Annotations.Add(annotation6);
        }
    }
}

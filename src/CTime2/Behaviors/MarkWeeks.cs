using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using CTime2.Views.Statistics.Details;
using Microsoft.Xaml.Interactivity;
using Telerik.UI.Xaml.Controls.Chart;

namespace CTime2.Behaviors
{
    public class MarkWeeks : Behavior<RadCartesianChart>
    {
        public static readonly DependencyProperty IsAddedFromProperty = DependencyProperty.RegisterAttached(
            "IsAddedFrom", typeof(MarkWeeks), typeof(MarkWeeks), new PropertyMetadata(default(MarkWeeks)));

        public static void SetIsAddedFrom(DependencyObject element, MarkWeeks value)
        {
            element.SetValue(IsAddedFromProperty, value);
        }

        public static MarkWeeks GetIsAddedFrom(DependencyObject element)
        {
            return (MarkWeeks) element.GetValue(IsAddedFromProperty);
        }

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
            nameof(StartDate), typeof(DateTimeOffset), typeof(MarkWeeks), new PropertyMetadata(default(DateTimeOffset), OnStartDateChanged));
        
        public DateTimeOffset StartDate
        {
            get { return (DateTimeOffset)this.GetValue(StartDateProperty); }
            set { this.SetValue(StartDateProperty, value); }
        }

        private static void OnStartDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MarkWeeks) sender;
            self.UpdateAnnotations();
        }

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            nameof(EndDate), typeof(DateTimeOffset), typeof(MarkWeeks), new PropertyMetadata(default(DateTimeOffset), OnEndDateChanged));

        public DateTimeOffset EndDate
        {
            get { return (DateTimeOffset)this.GetValue(EndDateProperty); }
            set { this.SetValue(EndDateProperty, value); }
        }

        private static void OnEndDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MarkWeeks)sender;
            self.UpdateAnnotations();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.UpdateAnnotations();
        }

        private void UpdateAnnotations()
        {
            if (this.AssociatedObject == null ||
                this.StartDate == default(DateTimeOffset) ||
                this.EndDate == default(DateTimeOffset) ||
                this.EndDate.Date < this.StartDate.Date)
                return;
            
            //Remove existing annotations that have been added from this instance
            foreach (var annotation in this.AssociatedObject.Annotations)
            {
                if (GetIsAddedFrom(annotation) == this)
                {
                    SetIsAddedFrom(annotation, null);
                    this.AssociatedObject.Annotations.Remove(annotation);
                }
            }

            //Add annotations
            for (DateTime d = this.StartDate.Date; d <= this.EndDate.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
                {
                    var weekNumber = new GregorianCalendar().GetWeekOfYear(d, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
                    var annotation = new CartesianGridLineAnnotation
                    {
                        Axis = this.AssociatedObject.HorizontalAxis,
                        Value = d.AddHours(-12), //The days are exactly on 00:00, so we shift by -12 and 12 to be between 2 days
                        Stroke = new SolidColorBrush(Colors.DimGray),
                        Foreground = new SolidColorBrush(Colors.DimGray),
                        StrokeThickness = 1,
                        Label = "KW " + weekNumber,
                        LabelDefinition = new ChartAnnotationLabelDefinition
                        {
                            Location = ChartAnnotationLabelLocation.Right,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalOffset = 4,
                        }
                    };

                    SetIsAddedFrom(annotation, this);

                    this.AssociatedObject.Annotations.Add(annotation);
                }
            }
        }
    }
}
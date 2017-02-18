using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;
using Telerik.UI.Xaml.Controls.Chart;

namespace CTime2.Behaviors
{
    public class MarkWeekEnds : Behavior<RadCartesianChart>
    {
        public static readonly DependencyProperty IsAddedFromProperty = DependencyProperty.RegisterAttached(
            "IsAddedFrom", typeof(MarkWeekEnds), typeof(MarkWeekEnds), new PropertyMetadata(default(MarkWeekEnds)));

        public static void SetIsAddedFrom(DependencyObject element, MarkWeekEnds value)
        {
            element.SetValue(IsAddedFromProperty, value);
        }

        public static MarkWeekEnds GetIsAddedFrom(DependencyObject element)
        {
            return (MarkWeekEnds)element.GetValue(IsAddedFromProperty);
        }

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
            nameof(StartDate), typeof(DateTimeOffset), typeof(MarkWeekEnds), new PropertyMetadata(default(DateTimeOffset), OnStartDateChanged));

        public DateTimeOffset StartDate
        {
            get { return (DateTimeOffset)this.GetValue(StartDateProperty); }
            set { this.SetValue(StartDateProperty, value); }
        }

        private static void OnStartDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MarkWeekEnds)sender;
            self.UpdateAnnotations();
        }

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            nameof(EndDate), typeof(DateTimeOffset), typeof(MarkWeekEnds), new PropertyMetadata(default(DateTimeOffset), OnEndDateChanged));

        public DateTimeOffset EndDate
        {
            get { return (DateTimeOffset)this.GetValue(EndDateProperty); }
            set { this.SetValue(EndDateProperty, value); }
        }

        private static void OnEndDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MarkWeekEnds)sender;
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
                double? opacity = d.AddDays(2).DayOfWeek == DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek //Saturday
                    ? 0.25
                    : d.AddDays(1).DayOfWeek == DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek //Sunday
                        ? 0.5
                        : (double?)null;

                if (opacity == null)
                    continue;

                var annotation = (CartesianMarkedZoneAnnotation)XamlReader.Load(@"
<telerikChart:CartesianMarkedZoneAnnotation 
    xmlns:telerikChart=""using:Telerik.UI.Xaml.Controls.Chart"" 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
    Fill=""{ThemeResource SystemControlHighlightAccentBrush}"" />
");
                annotation.Opacity = opacity.Value;
                annotation.VerticalFrom = -1000000;
                annotation.VerticalTo = 1000000;
                annotation.HorizontalFrom = d.AddHours(-12); //The days are exactly on 00:00, so we shift by -12 and 12 to be between 2 days
                annotation.HorizontalTo = d.AddHours(12);
                
                SetIsAddedFrom(annotation, this);

                this.AssociatedObject.Annotations.Add(annotation);
            }
        }
    }
}
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class NumberAboveConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty CompareToProperty = DependencyProperty.Register(
            "CompareTo", typeof(double), typeof(NumberAboveConverter), new PropertyMetadata(default(double)));

        public double CompareTo
        {
            get { return (double) GetValue(CompareToProperty); }
            set { SetValue(CompareToProperty, value); }
        }

        public object EqualOrBigger { get; set; }
        public object Less { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double number;
            if (double.TryParse(value?.ToString(), out number) == false)
                return value;

            return number >= this.CompareTo
                ? this.EqualOrBigger
                : this.Less;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
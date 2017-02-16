using System;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class NumberAboveZeroConverter : IValueConverter
    {
        public object ZeroOrAbove { get; set; }
        public object LessThanZero { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal number;
            if (decimal.TryParse(value?.ToString(), out number) == false)
                return value;

            return number >= 0m
                ? this.ZeroOrAbove
                : this.LessThanZero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
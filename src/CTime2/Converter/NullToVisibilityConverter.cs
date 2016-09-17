using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isNull = value == null;

            if (value is string)
                isNull = string.IsNullOrWhiteSpace((string) value);

            return isNull 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
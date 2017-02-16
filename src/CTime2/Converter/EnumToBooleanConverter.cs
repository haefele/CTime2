using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? parameter : DependencyProperty.UnsetValue;
        }
    }
}
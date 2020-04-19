using System;
using UwCore.Extensions;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var strings = value as string[];
            return string.Join(", ", strings.EmptyIfNull());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

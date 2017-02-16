using System;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class HoursToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double == false)
                return value;

            double d = (double) value;

            return TimeSpan.FromHours(d).ToString(@"h\ \h\ m\ \m\i\n");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
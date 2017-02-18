using System;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class MinutesToTimeStringConverter : IValueConverter
    {
        public string Prefix { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double == false)
                return value;

            double d = (double)value;
            var t = TimeSpan.FromMinutes(d);

            if (t == TimeSpan.Zero)
                return string.Empty;

            return this.Prefix + Math.Round(t.TotalMinutes) + " min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
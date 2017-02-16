using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class DoubleToHorizontalSizeConverter : IValueConverter
    {
        public double Height { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double == false)
                return value;

            var dec = (double) value;

            return new Size(dec, this.Height);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Size == false)
                return value;

            var size = (Size) value;
            return size.Width;
        }
    }
}
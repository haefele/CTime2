using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CTime2.Converter
{
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Thickness == false)
                return value;

            var thickness = (Thickness)value;

            return (thickness.Left + thickness.Top + thickness.Right + thickness.Bottom)/4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
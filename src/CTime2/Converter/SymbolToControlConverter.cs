using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CTime2.Converter
{
    public class SymbolToControlConverter : IValueConverter
    {
        public double FontSize { get; set; } = 20d;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Symbol)
            {
                return new SymbolIcon((Symbol)value);
            }
            if (value is string)
            {
                return new TextBlock
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = this.FontSize,
                    Text = (string)value,
                };
            }
            if (value is char)
            {
                return new TextBlock
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = this.FontSize,
                    Text = ((char)value).ToString(),
                };
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
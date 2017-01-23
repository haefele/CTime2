using System;
using Windows.UI.Xaml.Data;
using ByteArrayExtensions = CTime2.Extensions.ByteArrayExtensions;

namespace CTime2.Converter
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var bytes = value as byte[];
            return ByteArrayExtensions.ToImage(bytes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

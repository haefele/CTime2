using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using UwCore.Extensions;
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

    public class FeatureToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FeatureNameProperty = DependencyProperty.Register(
            nameof(FeatureName), 
            typeof(string), 
            typeof(FeatureToVisibilityConverter), 
            new PropertyMetadata(default(string)));

        public string FeatureName
        {
            get { return (string)this.GetValue(FeatureNameProperty); }
            set { this.SetValue(FeatureNameProperty, value); }
        }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var propertyInfo = typeof(Features).GetPropertyCaseInsensitive(this.FeatureName);
            var isEnabled = (bool)propertyInfo.GetValue(null);

            return isEnabled 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}

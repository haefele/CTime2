using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using UwCore.Extensions;

namespace CTime2.Converter
{
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
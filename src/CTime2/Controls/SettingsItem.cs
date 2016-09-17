using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Controls
{
    public class SettingsItem : ContentControl
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(SettingsItem), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string) this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        public SettingsItem()
        {
            this.DefaultStyleKey = typeof(SettingsItem);
        }
    }
}
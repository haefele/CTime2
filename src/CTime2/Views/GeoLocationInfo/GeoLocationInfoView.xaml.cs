using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.GeoLocationInfo
{
    public sealed partial class GeoLocationInfoView : Page
    {
        public GeoLocationInfoView()
        {
            this.InitializeComponent();
        }

        private async void OpenSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }
    }
}

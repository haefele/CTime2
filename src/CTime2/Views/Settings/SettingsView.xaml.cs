using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;

        public SettingsView()
        {
            this.InitializeComponent();
        }
    }
}

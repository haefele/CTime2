using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Licenses
{
    public sealed partial class LicenseView : Page
    {
        public LicenseViewModel ViewModel => this.DataContext as LicenseViewModel;

        public LicenseView()
        {
            this.InitializeComponent();
        }
        
        private void LicenseView_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.LicenseWebView.NavigateToString(this.ViewModel.LicenseText);
        }
    }
}

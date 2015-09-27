using Windows.UI.Xaml.Controls;

namespace CTime2.Views.About
{
    public sealed partial class AboutView : Page
    {
        public AboutViewModel ViewModel => this.DataContext as AboutViewModel;

        public AboutView()
        {
            this.InitializeComponent();
        }
    }
}

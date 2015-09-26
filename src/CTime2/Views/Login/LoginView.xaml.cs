using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Login
{
    public sealed partial class LoginView : Page
    {
        public LoginView()
        {
            this.InitializeComponent();
        }

        public LoginViewModel ViewModel => this.DataContext as LoginViewModel;
    }
}

using Windows.UI.Xaml.Controls;

namespace CTime2.Views.StampTime.HomeOfficeCheckedIn
{
    public sealed partial class HomeOfficeCheckedInView : UserControl
    {
        public HomeOfficeCheckedInViewModel TimeStateViewModel => this.DataContext as HomeOfficeCheckedInViewModel;

        public HomeOfficeCheckedInView()
        {
            this.InitializeComponent();
        }
    }
}

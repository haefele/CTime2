using Windows.UI.Xaml.Controls;

namespace CTime2.Views.AttendanceList
{
    public sealed partial class AttendingUserDetailsView : Page
    {
        public AttendingUserDetailsViewModel ViewModel => this.DataContext as AttendingUserDetailsViewModel;

        public AttendingUserDetailsView()
        {
            this.InitializeComponent();
        }
    }
}

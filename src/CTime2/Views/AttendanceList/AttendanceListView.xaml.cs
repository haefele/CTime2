using Windows.UI.Xaml.Controls;

namespace CTime2.Views.AttendanceList
{
    public sealed partial class AttendanceListView : Page
    {
        public AttendanceListView()
        {
            this.InitializeComponent();
        }

        public AttendanceListViewModel ViewModel => this.DataContext as AttendanceListViewModel;
    }
}

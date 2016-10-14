using Windows.UI.Xaml.Controls;
using CTime2.Core.Data;

namespace CTime2.Views.AttendanceList
{
    public sealed partial class AttendanceListView : Page
    {
        public AttendanceListView()
        {
            this.InitializeComponent();
        }

        public AttendanceListViewModel ViewModel => this.DataContext as AttendanceListViewModel;

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var user = (AttendingUser) e.ClickedItem;
            this.ViewModel.SelectedUser = user;

            await this.ViewModel.ShowDetails.ExecuteAsync();
        }
    }
}

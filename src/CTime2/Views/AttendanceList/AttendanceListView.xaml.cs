using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using CTime2.Core.Data;

namespace CTime2.Views.AttendanceList
{
    public sealed partial class AttendanceListView : Page
    {
        public AttendanceListView()
        {
            this.InitializeComponent();
            
            //Fix flyout not working on anniversary update
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
                this.SaveGroupAppBarButton.AllowFocusOnInteraction = true;
        }

        public AttendanceListViewModel ViewModel => this.DataContext as AttendanceListViewModel;

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var user = (AttendingUser) e.ClickedItem;

            this.ViewModel.SelectedUsers.Clear();
            this.ViewModel.SelectedUsers.Add(user);

            await this.ViewModel.ShowDetails.ExecuteAsync();
        }

        private void GridView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.SelectedUsers.Clear();
            this.ViewModel.SelectedUsers.AddRange(this.AttendanceListGridView.SelectedItems.OfType<AttendingUser>());
        }
    }
}

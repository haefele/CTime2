using Windows.UI.Xaml.Controls;
using CTime2.Core.Data;

namespace CTime2.Views.Settings.Licenses
{
    public sealed partial class LicensesListView
    {
        public LicensesListViewModel ViewModel => this.DataContext as LicensesListViewModel;

        public LicensesListView()
        {
            this.InitializeComponent();
        }

        private void LicensesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var license = (License)e.ClickedItem;
            this.ViewModel.ShowLicense(license);
        }
    }
}

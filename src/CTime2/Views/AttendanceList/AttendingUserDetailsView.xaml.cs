using Windows.UI.Xaml;
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

        private async void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.AddNotify.CanExecute)
                await this.ViewModel.AddNotify.ExecuteAsync();
        }

        private async void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.RemoveNotify.CanExecute)
                await this.ViewModel.RemoveNotify.ExecuteAsync();
        }
    }
}

using Windows.UI.Xaml.Controls;

namespace CTime2.Views.YourTimes
{
    public sealed partial class YourTimesView : Page
    {
        public YourTimesViewModel ViewModel => this.DataContext as YourTimesViewModel;

        public YourTimesView()
        {
            this.InitializeComponent();
        }

        private async void EndDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            await this.ViewModel.RefreshAsync();
        }

        private async void StartDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            await this.ViewModel.RefreshAsync();
        }
    }
}

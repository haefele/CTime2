using Windows.UI.Xaml.Controls;

namespace CTime2.Views.AddVacation
{
    public sealed partial class AddVacationView : Page
    {
        public AddVacationView()
        {
            this.InitializeComponent();
        }

        public AddVacationViewModel ViewModel => this.DataContext as AddVacationViewModel;
    }
}

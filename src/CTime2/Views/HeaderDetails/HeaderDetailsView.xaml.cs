using Windows.UI.Xaml.Controls;

namespace CTime2.Views.HeaderDetails
{
    public sealed partial class HeaderDetailsView : UserControl
    {
        public HeaderDetailsViewModel ViewModel => this.DataContext as HeaderDetailsViewModel;

        public HeaderDetailsView()
        {
            this.InitializeComponent();
        }
    }
}

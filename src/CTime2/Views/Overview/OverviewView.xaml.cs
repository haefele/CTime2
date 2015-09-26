using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Overview
{
    public sealed partial class OverviewView : Page
    {
        public OverviewView()
        {
            this.InitializeComponent();
        }

        public OverviewViewModel ViewModel => this.DataContext as OverviewViewModel;
    }
}

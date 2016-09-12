using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CTime2.Views.Overview
{
    public sealed partial class OverviewView : Page
    {
        public OverviewViewModel ViewModel => this.DataContext as OverviewViewModel;

        public OverviewView()
        {
            this.InitializeComponent();
        }
    }
}

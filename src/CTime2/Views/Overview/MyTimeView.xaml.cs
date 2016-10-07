using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Overview
{
    public sealed partial class MyTimeView : Page
    {
        public MyTimeViewModel ViewModel => this.DataContext as MyTimeViewModel;

        public MyTimeView()
        {
            this.InitializeComponent();
        }
    }
}

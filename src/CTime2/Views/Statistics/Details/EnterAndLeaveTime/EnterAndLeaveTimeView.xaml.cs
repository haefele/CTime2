using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.Details.EnterAndLeaveTime
{
    public sealed partial class EnterAndLeaveTimeView : Page
    {
        public EnterAndLeaveTimeViewModel ViewModel => this.DataContext as EnterAndLeaveTimeViewModel;

        public EnterAndLeaveTimeView()
        {
            this.InitializeComponent();
        }
    }
}

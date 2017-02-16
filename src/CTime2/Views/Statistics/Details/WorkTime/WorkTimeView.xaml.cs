using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.Details.WorkTime
{
    public sealed partial class WorkTimeView : Page
    {
        public WorkTimeViewModel ViewModel => this.DataContext as WorkTimeViewModel;

        public WorkTimeView()
        {
            this.InitializeComponent();
        }
    }
}

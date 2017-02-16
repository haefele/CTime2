using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Statistics.Details.BreakTime
{
    public sealed partial class BreakTimeView : Page
    {
        public BreakTimeViewModel ViewModel => this.DataContext as BreakTimeViewModel;

        public BreakTimeView()
        {
            this.InitializeComponent();
        }
    }
}

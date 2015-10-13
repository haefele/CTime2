namespace CTime2.Views.StampTime
{
    public sealed partial class TripCheckedInView
    {
        public TripCheckedInViewModel TimeStateViewModel => this.DataContext as TripCheckedInViewModel;

        public TripCheckedInView()
        {
            this.InitializeComponent();
        }
    }
}

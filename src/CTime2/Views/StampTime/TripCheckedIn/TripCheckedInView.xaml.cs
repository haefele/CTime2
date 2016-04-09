namespace CTime2.Views.StampTime.TripCheckedIn
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

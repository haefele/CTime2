namespace CTime2.Views.Settings.Band
{
    public sealed partial class BandView
    {
        public BandViewModel ViewModel => this.DataContext as BandViewModel;

        public BandView()
        {
            this.InitializeComponent();
        }
    }
}

namespace CTime2.Views.Settings.Others
{
    public sealed partial class OthersView
    {
        public OthersViewModel ViewModel => this.DataContext as OthersViewModel;

        public OthersView()
        {
            this.InitializeComponent();
        }
    }
}

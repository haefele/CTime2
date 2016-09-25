namespace CTime2.Views.About
{
    public sealed partial class AboutView
    {
        public AboutViewModel ViewModel => this.DataContext as AboutViewModel;

        public AboutView()
        {
            this.InitializeComponent();
        }
    }
}

namespace CTime2.Views.Overview.CheckedIn
{
    public sealed partial class CheckedInView
    {
        public CheckedInViewModel TimeStateViewModel => this.DataContext as CheckedInViewModel;

        public CheckedInView()
        {
            this.InitializeComponent();
        }
    }
}

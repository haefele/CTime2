namespace CTime2.Views.Overview.CheckedOut
{
    public sealed partial class CheckedOutView
    {
        public CheckedOutViewModel TimeStateViewModel => this.DataContext as CheckedOutViewModel;

        public CheckedOutView()
        {
            this.InitializeComponent();
        }
    }
}

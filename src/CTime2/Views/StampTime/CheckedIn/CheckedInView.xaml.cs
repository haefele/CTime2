namespace CTime2.Views.StampTime.CheckedIn
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

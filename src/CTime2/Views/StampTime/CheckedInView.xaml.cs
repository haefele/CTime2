namespace CTime2.Views.StampTime
{
    public sealed partial class CheckedInView
    {
        public CheckedInViewModel ViewModel => this.DataContext as CheckedInViewModel;

        public CheckedInView()
        {
            this.InitializeComponent();
        }
    }
}

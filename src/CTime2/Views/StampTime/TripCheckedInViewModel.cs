using Caliburn.Micro;

namespace CTime2.Views.StampTime
{
    public class TripCheckedInViewModel : Screen
    {
        public StampTimeViewModel Container => this.Parent as StampTimeViewModel;
    }
}
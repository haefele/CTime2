using Caliburn.Micro.ReactiveUI;

namespace CTime2.Views.AttendanceList
{
    public class AttendingUserDetailsViewModel : ReactiveScreen
    {
        public string AttendingUserId { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
    }
}
using CTime2.Services.CTime;

namespace CTime2.Services.SessionState
{
    public class SessionStateService : ISessionStateService
    {
        public User CurrentUser { get; set; }
    }
}
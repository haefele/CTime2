using Caliburn.Micro;
using CTime2.Services.SessionState;

namespace CTime2.Views.Overview
{
    public class OverviewViewModel : Screen
    {
        private readonly ISessionStateService _sessionStateService;

        public OverviewViewModel(ISessionStateService sessionStateService)
        {
            this._sessionStateService = sessionStateService;

            this.WelcomeMessage = $"Hallo {this._sessionStateService.CurrentUser.FirstName}";
        }

        public string WelcomeMessage { get; set; }

    }
}
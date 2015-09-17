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

            this.WelcomeMessage = $"Hello {this._sessionStateService.CurrentUser.Name}";
        }

        public string WelcomeMessage { get; set; }

    }
}
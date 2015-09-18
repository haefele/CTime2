﻿using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Services.SessionState;
using CTime2.Views.Overview;
using CTime2.Views.StampTime;
using CTime2.Views.YourTimes;

namespace CTime2.Views.Shell.States
{
    public class LoggedInShellState : ShellState
    {
        private readonly INavigationService _navigationService;
        private readonly ISessionStateService _sessionStateService;

        private readonly NavigationItemViewModel _overviewNavigationItem;
        private readonly NavigationItemViewModel _myTimesNavigationItem;
        private readonly NavigationItemViewModel _stampTimeNavigationItem;
        private readonly NavigationItemViewModel _logoutNavigationItem;

        public LoggedInShellState(INavigationService navigationService, ISessionStateService sessionStateService)
        {
            this._navigationService = navigationService;
            this._sessionStateService = sessionStateService;

            this._overviewNavigationItem = new NavigationItemViewModel(this.Overview) { Label = "Übersicht", Symbol = Symbol.Globe };
            this._myTimesNavigationItem = new NavigationItemViewModel(this.MyTimes) { Label = "Meine Zeiten", Symbol = Symbol.Calendar };
            this._stampTimeNavigationItem = new NavigationItemViewModel(this.StampTime) { Label = "Stempeln", Symbol = Symbol.Clock };
            this._logoutNavigationItem = new NavigationItemViewModel(this.Logout) { Label = "Abmelden", Symbol = Symbol.LeaveChat };
        }

        public override void Enter()
        {
            this.ViewModel.Actions.Add(this._overviewNavigationItem);
            this.ViewModel.Actions.Add(this._myTimesNavigationItem);
            this.ViewModel.Actions.Add(this._stampTimeNavigationItem);
            this.ViewModel.SecondaryActions.Add(this._logoutNavigationItem);

            this.Overview();
        }

        public override void Leave()
        {
            this.ViewModel.Actions.Remove(this._overviewNavigationItem);
            this.ViewModel.Actions.Remove(this._myTimesNavigationItem);
            this.ViewModel.Actions.Remove(this._stampTimeNavigationItem);
            this.ViewModel.SecondaryActions.Remove(this._logoutNavigationItem);
        }

        private void Overview()
        {
            this._navigationService
                .For<OverviewViewModel>()
                .Navigate();
        }

        private void MyTimes()
        {
            this._navigationService
                .For<YourTimesViewModel>()
                .Navigate();
        }

        private void StampTime()
        {
            this._navigationService
                .For<StampTimeViewModel>()
                .Navigate();
        }

        private void Logout()
        {
            this._sessionStateService.CompanyId = null;
            this._sessionStateService.CurrentUser = null;

            this.ViewModel.CurrentState = IoC.Get<LoggedOutShellState>();
        }
    }
}
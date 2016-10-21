﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Common;
using CTime2.Core.Data;
using CTime2.Core.Events;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.EmployeeGroups;
using CTime2.Strings;
using CTime2.Views.AttendanceList;
using CTime2.Views.Overview;
using CTime2.Views.Statistics;
using CTime2.Views.YourTimes;
using Microsoft.HockeyApp;
using UwCore.Application;
using UwCore.Common;
using UwCore.Events;
using UwCore.Hamburger;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;

namespace CTime2.ApplicationModes
{
    [AutoSubscribeEvents]
    public class LoggedInApplicationMode : ApplicationMode, ICustomStartupApplicationMode, IHandleWithTask<EmployeeGroupCreated>, IHandleWithTask<EmployeeGroupDeleted>
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IDialogService _dialogService;
        private readonly ICTimeService _cTimeService;
        private readonly IEmployeeGroupService _employeeGroupService;

        private readonly HamburgerItem _overviewHamburgerItem;
        private readonly HamburgerItem _myTimesHamburgerItem;
        private readonly HamburgerItem _attendanceListHamburgerItem;
        private readonly IList<HamburgerItem> _employeeGroupHamburgerItems;
        private readonly HamburgerItem _statisticsItem;
        private readonly HamburgerItem _logoutHamburgerItem;

        public LoggedInApplicationMode(IApplicationStateService applicationStateService, IDialogService dialogService, ICTimeService cTimeService, IEmployeeGroupService employeeGroupService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(dialogService, nameof(dialogService));
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(employeeGroupService, nameof(employeeGroupService));

            this._applicationStateService = applicationStateService;
            this._dialogService = dialogService;
            this._cTimeService = cTimeService;
            this._employeeGroupService = employeeGroupService;

            this._overviewHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Overview"), Symbol.Globe, typeof(OverviewViewModel));
            this._myTimesHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.MyTimes"), Symbol.Calendar, typeof(YourTimesViewModel));
            this._attendanceListHamburgerItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.AttendanceList"), SymbolEx.AttendanceList, typeof(AttendanceListViewModel));
            this._employeeGroupHamburgerItems = new List<HamburgerItem>();
            this._statisticsItem = new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Statistics"), SymbolEx.Statistics, typeof(StatisticsViewModel));

            this._logoutHamburgerItem = new ClickableHamburgerItem(CTime2Resources.Get("Navigation.Logout"), SymbolEx.Logout, this.Logout);
        }
        
        protected override async Task OnEnter()
        {
            await base.OnEnter();

            var currentUser = this._applicationStateService.GetCurrentUser();
            HockeyClient.Current.UpdateContactInfo($"{currentUser.FirstName} {currentUser.Name}", currentUser.Email);

            this._overviewHamburgerItem.Execute();
        }
        
        protected override async Task AddActions()
        {
            await base.AddActions();

            this.Application.Actions.Add(this._overviewHamburgerItem);
            this.Application.Actions.Add(this._myTimesHamburgerItem);
            this.Application.Actions.Add(this._attendanceListHamburgerItem);

            var currentUser = this._applicationStateService.GetCurrentUser();
            foreach (var group in await this._employeeGroupService.GetGroupsAsync(currentUser.Id))
            {
                var hamburgerItem = new NavigatingHamburgerItem(@group.Name, SymbolEx.AttendanceList, typeof(AttendanceListViewModel));
                hamburgerItem.AddParameter<AttendanceListViewModel>(f => f.EmployeeGroupId, @group.Id);

                this._employeeGroupHamburgerItems.Add(hamburgerItem);
                this.Application.Actions.Add(hamburgerItem);
            }

            this.Application.Actions.Add(this._statisticsItem);
            this.Application.SecondaryActions.Add(this._logoutHamburgerItem);
        }

        protected override async Task RemoveActions()
        {
            await base.RemoveActions();

            this.Application.Actions.Remove(this._overviewHamburgerItem);
            this.Application.Actions.Remove(this._myTimesHamburgerItem);
            this.Application.Actions.Remove(this._attendanceListHamburgerItem);
            this.Application.Actions.RemoveRange(this._employeeGroupHamburgerItems);
            this.Application.Actions.Remove(this._statisticsItem);
            this.Application.SecondaryActions.Remove(this._logoutHamburgerItem);
        }
        
        private async void Logout()
        {
            this._applicationStateService.SetCurrentUser(null);
            await this._applicationStateService.SaveStateAsync();

            this.Application.CurrentMode = IoC.Get<LoggedOutApplicationMode>();
        }

        public async void HandleCustomStartup(string tileId, string arguments)
        {
            var parsed = StartupArguments.Parse<CTimeStartupArguments>(arguments);

            if (parsed == null)
                return;

            switch (parsed.Action)
            {
                case CTimeStartupAction.Checkin:
                    await this.StampAsync(TimeState.Entered);
                    break;
                case CTimeStartupAction.Checkout:
                    await this.StampAsync(TimeState.Left);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task StampAsync(TimeState timeState)
        {
            var stampHelper = new CTimeStampHelper(this._applicationStateService, this._cTimeService);
            var callbackHandler = new CTimeStampHelperCallback(
                this.OnNotLoggedIn,
                this.SupportsQuestions,
                this.OnDidNothing,
                this.OnAlreadyCheckedInWannaCheckOut,
                this.OnAlreadyCheckedIn,
                this.OnAlreadyCheckedOutWannaCheckIn,
                this.OnAlreadyCheckedOut,
                this.OnSuccess);

            await stampHelper.Stamp(callbackHandler, timeState);
        }

        #region StampHelper Callbacks
        private Task OnNotLoggedIn()
        {
            throw new NotImplementedException();
        }

        private bool SupportsQuestions()
        {
            return true;
        }

        private Task OnDidNothing()
        {
            return Task.CompletedTask;
        }

        private async Task<bool> OnAlreadyCheckedInWannaCheckOut()
        {
            var checkOutCommand = new UICommand(CTime2Resources.Get("StampHelper.CheckOut"));
            var noCommand = new UICommand(CTime2Resources.Get("StampHelper.No"));

            string message = CTime2Resources.Get("StampHelper.AlreadyCheckedInWannaCheckOutMessage");
            string title = CTime2Resources.Get("StampHelper.AlreadyCheckedInWannaCheckOutTitle");

            var selectedCommand = await this._dialogService.ShowAsync(message, title, new List<UICommand> { checkOutCommand, noCommand });

            return selectedCommand == checkOutCommand;
        }

        private Task OnAlreadyCheckedIn()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> OnAlreadyCheckedOutWannaCheckIn()
        {
            var checkInCommand = new UICommand(CTime2Resources.Get("StampHelper.CheckIn"));
            var noCommand = new UICommand(CTime2Resources.Get("StampHelper.No"));

            string message = CTime2Resources.Get("StampHelper.AlreadyCheckedOutWannaCheckInMessage");
            string title = CTime2Resources.Get("StampHelper.AlreadyCheckedOutWannaCheckInTitle");

            var selectedCommand = await this._dialogService.ShowAsync(message, title, new List<UICommand> { checkInCommand, noCommand });

            return selectedCommand == checkInCommand;
        }

        private Task OnAlreadyCheckedOut()
        {
            throw new NotImplementedException();
        }

        private Task OnSuccess(TimeState arg)
        {
            return Task.CompletedTask;
        }
        #endregion
        
        #region Events
        async Task IHandleWithTask<EmployeeGroupCreated>.Handle(EmployeeGroupCreated message)
        {
            await this.RefreshActions();
        }

        async Task IHandleWithTask<EmployeeGroupDeleted>.Handle(EmployeeGroupDeleted message)
        {
            await this.RefreshActions();
        }
        #endregion
    }
}
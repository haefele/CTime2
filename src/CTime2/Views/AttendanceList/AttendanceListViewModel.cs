using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials.UI;
using Windows.UI.Popups;
using CTime2.Core.Data;
using CTime2.Core.Services.ApplicationState;
using CTime2.Core.Services.CTime;
using CTime2.Core.Services.EmployeeGroups;
using CTime2.Extensions;
using CTime2.Strings;
using ReactiveUI;
using UwCore;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Services.Navigation;

namespace CTime2.Views.AttendanceList
{
    public class AttendanceListViewModel : UwCoreScreen
    {
        private readonly ICTimeService _cTimeService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeGroupService _employeeGroupService;
        private readonly IDialogService _dialogService;

        private ReactiveList<AttendingUser> _selectedUsers;
        private readonly ObservableAsPropertyHelper<ReactiveList<AttendingUserByIsAttending>> _usersHelper;
        private readonly ObservableAsPropertyHelper<ReactiveList<AttendingUserByIsAttending>> _filteredUsersHelper;
        private AttendanceListState _state;
        private string _groupName;
        private string _searchText;

        public ReactiveList<AttendingUser> SelectedUsers
        {
            get { return this._selectedUsers; }
            set { this.RaiseAndSetIfChanged(ref this._selectedUsers, value); }
        }
        public ReactiveList<AttendingUserByIsAttending> Users => this._usersHelper.Value;
        public ReactiveList<AttendingUserByIsAttending> FilteredUsers => this._filteredUsersHelper.Value;
        public AttendanceListState State
        {
            get { return this._state; }
            set { this.RaiseAndSetIfChanged(ref this._state, value); }
        }
        public string GroupName
        {
            get { return this._groupName; }
            set { this.RaiseAndSetIfChanged(ref this._groupName, value); }
        }
        public string SearchText
        {
            get { return this._searchText; }
            set { this.RaiseAndSetIfChanged(ref this._searchText, value); }
        }

        public UwCoreCommand<ReactiveList<AttendingUserByIsAttending>> LoadUsers { get; }
        public UwCoreCommand<ReactiveList<AttendingUserByIsAttending>> FilterUsers { get; }
        public UwCoreCommand<Unit> ShowDetails { get; }
        public UwCoreCommand<Unit> CreateGroup { get; }
        public UwCoreCommand<Unit> SaveGroup { get; }
        public UwCoreCommand<Unit> CancelCreateGroup { get; }
        public UwCoreCommand<Unit> DeleteGroup { get; }

        #region Parameters
        public string EmployeeGroupId { get; set; }
        #endregion

        public AttendanceListViewModel(ICTimeService cTimeService, IApplicationStateService applicationStateService, INavigationService navigationService, IEmployeeGroupService employeeGroupService, IDialogService dialogService)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(applicationStateService, nameof(applicationStateService));
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(employeeGroupService, nameof(employeeGroupService));
            Guard.NotNull(dialogService, nameof(dialogService));

            this._cTimeService = cTimeService;
            this._applicationStateService = applicationStateService;
            this._navigationService = navigationService;
            this._employeeGroupService = employeeGroupService;
            this._dialogService = dialogService;

            this.DisplayName = CTime2Resources.Get("Navigation.AttendanceList");
            this.SelectedUsers = new ReactiveList<AttendingUser>();
            this.State = AttendanceListState.Loading;

            this.LoadUsers = UwCoreCommand.Create(this.LoadUsersImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.AttendanceList"))
                .HandleExceptions()
                .TrackEvent("LoadAttendanceList");
            this.LoadUsers.ToProperty(this, f => f.Users, out this._usersHelper);

            this.FilterUsers = UwCoreCommand.Create(this.FilterUsersImpl)
                .HandleExceptions();
            this.FilterUsers.ToProperty(this, f => f.FilteredUsers, out this._filteredUsersHelper);

            this.WhenAnyValue(f => f.Users, f => f.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .InvokeCommand(this.FilterUsers);

            var canShowDetails = this.SelectedUsers.Changed.Select(f => this.SelectedUsers.Any());
            this.ShowDetails = UwCoreCommand.Create(canShowDetails, this.ShowDetailsImpl)
                .HandleExceptions()
                .TrackEvent("ShowAttendingUserDetails");

            var canCreateGroup = this.WhenAnyValue(f => f.State, mode => mode == AttendanceListState.View);
            this.CreateGroup = UwCoreCommand.Create(canCreateGroup, this.CreateGroupImpl)
                .HandleExceptions()
                .TrackEvent("CreateNewEmployeeGroup");

            var canSaveGroup = this
                .WhenAnyValue(f => f.State, f => f.GroupName,  (mode, groupName) => mode == AttendanceListState.CreateGroup && string.IsNullOrWhiteSpace(groupName) == false)
                .CombineLatest(this.SelectedUsers.Changed, (stateAndName, selectedUsers) => stateAndName && this.SelectedUsers.Any());
            this.SaveGroup = UwCoreCommand.Create(canSaveGroup, this.SaveGroupImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.SaveEmployeeGroup"))
                .HandleExceptions()
                .TrackEvent("SaveNewEmployeeGroup");

            var canCancelCreateGroup = this.WhenAnyValue(f => f.State, mode => mode == AttendanceListState.CreateGroup);
            this.CancelCreateGroup = UwCoreCommand.Create(canCancelCreateGroup, this.CancelCreateGroupImpl)
                .HandleExceptions()
                .TrackEvent("CancelCreateNewEmployeeGroup");

            var canDeleteGroup = this.WhenAnyValue(f => f.State, mode => mode == AttendanceListState.ViewGroup);
            this.DeleteGroup = UwCoreCommand.Create(canDeleteGroup, this.DeleteGroupImpl)
                .ShowLoadingOverlay(CTime2Resources.Get("Loading.DeleteEmployeeGroup"))
                .HandleExceptions()
                .TrackEvent("DeleteEmployeeGroup");
        }

        private Task<ReactiveList<AttendingUserByIsAttending>> FilterUsersImpl()
        {
            return Task.Run(() =>
            {
                if (this.Users == null)
                    return null;

                var searchText = (this.SearchText ?? string.Empty).Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);

                var filteredUsers = this.Users
                    .SelectMany(f => f.Users)
                    .Where(f => searchText.All(s => f.FirstName.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                                                    f.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                                                    f.EmailAddress.Contains(s, StringComparison.OrdinalIgnoreCase)));

                var result = new ReactiveList<AttendingUserByIsAttending>(AttendingUserByIsAttending.Create(filteredUsers));
                
                //If the lists didn't change, just return the same value as before
                return new ReactiveListOfAttendingUserByIsAttendingEqualityComparer().Equals(result, this.FilteredUsers)
                    ? this.FilteredUsers 
                    : result;
            });
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            await this.LoadUsers.ExecuteAsync();
            await this.FilterUsers.ExecuteAsync();
        }

        private async Task<ReactiveList<AttendingUserByIsAttending>> LoadUsersImpl()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();

            var attendingUsers = await this._cTimeService.GetAttendingUsers(currentUser.CompanyId, currentUser.CompanyImageAsPng);

            var employeeGroup = string.IsNullOrWhiteSpace(this.EmployeeGroupId) == false
                ? await this._employeeGroupService.GetGroupAsync(currentUser.Id, this.EmployeeGroupId)
                : null;

            if (employeeGroup != null)
            {
                this.State = AttendanceListState.ViewGroup;
                this.DisplayName = employeeGroup.Name;

                attendingUsers = attendingUsers
                    .Where(f => employeeGroup.EmployeeIds.Contains(f.Id))
                    .ToList();
            }
            else
            {
                this.State = AttendanceListState.View;
            }

            return new ReactiveList<AttendingUserByIsAttending>(AttendingUserByIsAttending.Create(attendingUsers));
        }

        private Task ShowDetailsImpl()
        {
            this._navigationService.Popup
                .For<AttendingUserDetailsViewModel>()
                .WithParam(f => f.Parameter.AttendingUserId, this.SelectedUsers.First().Id)
                .Navigate();

            return Task.CompletedTask;
        }

        private async Task CreateGroupImpl()
        {
            this.SearchText = string.Empty;
            await this.FilterUsers.ExecuteAsync();

            this.State = AttendanceListState.CreateGroup;
        }

        private async Task SaveGroupImpl()
        {
            var currentUser = this._applicationStateService.GetCurrentUser();

            await this._employeeGroupService.CreateGroupAsync(currentUser.Id, this.GroupName, this.SelectedUsers.Select(f => f.Id).ToList());

            await this.CancelCreateGroup.ExecuteAsync();
        }

        private Task CancelCreateGroupImpl()
        {
            this.GroupName = string.Empty;
            this.SelectedUsers.Clear();

            this.State = AttendanceListState.View;

            return Task.CompletedTask;
        }

        private async Task DeleteGroupImpl()
        {
            var deleteGroupCommand = new UICommand(CTime2Resources.Get("DeleteGroup.YesCommand"));
            var noCommand = new UICommand(CTime2Resources.Get("DeleteGroup.NoCommand"));

            var message = CTime2Resources.Get("DeleteGroup.Message");
            var title = CTime2Resources.Get("DeleteGroup.Title");

            var selectedCommand = await this._dialogService.ShowAsync(message, title, new[] {deleteGroupCommand, noCommand});

            if (selectedCommand == noCommand)
                return;

            var currentUser = this._applicationStateService.GetCurrentUser();

            await this._employeeGroupService.DeleteGroupAsync(currentUser.Id, this.EmployeeGroupId);

            this._navigationService.For<AttendanceListViewModel>().Navigate();
        }
    }
}
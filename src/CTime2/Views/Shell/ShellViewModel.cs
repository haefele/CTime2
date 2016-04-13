using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Events;
using CTime2.Extensions;
using CTime2.Services.Navigation;
using CTime2.States;
using CTime2.Strings;
using CTime2.Views.Settings;

namespace CTime2.Views.Shell
{
    public class ShellViewModel : Screen, IApplication, IHandle<NavigatedEvent>
    {
        private readonly ICTimeNavigationService _navigationService;

        private HamburgerItem _selectedAction;
        private HamburgerItem _selectedSecondaryAction;
        private ApplicationState _currentState;

        private object _latestViewModel;

        public BindableCollection<HamburgerItem> Actions { get; }

        public HamburgerItem SelectedAction
        {
            get { return this._selectedAction; }
            set { this.SetProperty(ref this._selectedAction, value); }
        }

        public BindableCollection<HamburgerItem> SecondaryActions { get; }

        public HamburgerItem SelectedSecondaryAction
        {
            get { return this._selectedSecondaryAction; }
            set { this.SetProperty(ref this._selectedSecondaryAction, value); }
        }

        public ApplicationState CurrentState
        {
            get { return this._currentState; }
            set
            {
                this._currentState?.Leave();

                this._currentState = value;
                this._currentState.Application = this;

                this._currentState?.Enter();

                this._navigationService.ClearBackStack();
            }
        }

        public ShellViewModel(ICTimeNavigationService navigationService, IEventAggregator eventAggregator)
        {
            this._navigationService = navigationService;

            this.Actions = new BindableCollection<HamburgerItem>();
            this.SecondaryActions = new BindableCollection<HamburgerItem>
            {
                new NavigatingHamburgerItem(CTime2Resources.Get("Navigation.Settings"), Symbol.Setting, typeof(SettingsViewModel)),
            };

            //Just make sure the selected action is always correct
            //Because it might happen, that we navigate to some view-model and then after that update the actions
            this.Actions.CollectionChanged += (s, e) => this.UpdateSelectedAction();
            this.SecondaryActions.CollectionChanged += (s, e) => this.UpdateSelectedAction();

            eventAggregator.Subscribe(this);
        }
        
        private void UpdateSelectedAction()
        {
            var selectedAction = this.Actions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel));

            var selectedSecondaryAction = this.SecondaryActions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel));

            if (selectedAction != null || selectedSecondaryAction != null)
            {
                this.SelectedAction = selectedAction;
                this.SelectedSecondaryAction = selectedSecondaryAction;
            }
        }
        
        void IHandle<NavigatedEvent>.Handle(NavigatedEvent message)
        {
            this._latestViewModel = message.ViewModel;
            this.UpdateSelectedAction();
        }

        public void ExecuteAction(HamburgerItem hamburgerItem)
        {
            var navigating = hamburgerItem as NavigatingHamburgerItem;
            if (navigating != null)
            {
                this._navigationService.Navigate(navigating.ViewModelType);
            }

            var clickable = hamburgerItem as ClickableHamburgerItem;
            if (clickable != null)
            {
                clickable.Action();
            }
        }
    }
}
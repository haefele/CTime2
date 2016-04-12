using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using CTime2.Events;

namespace CTime2.Services.Navigation
{
    public class CTimeNavigationService : FrameAdapter, ICTimeNavigationService
    {
        private readonly IEventAggregator _eventAggregator;

        public CTimeNavigationService(Frame frame, IEventAggregator eventAggregator, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this._eventAggregator = eventAggregator;
        }

        public NavigateHelper<TViewModel> For<TViewModel>()
        {
            return new NavigateHelper<TViewModel>().AttachTo(this);
        }

        public void ClearBackStack()
        {
            this.BackStack.Clear();
            this.UpdateAppViewBackButtonVisibility();
        }
        
        protected override void OnNavigated(object sender, NavigationEventArgs e)
        {
            base.OnNavigated(sender, e);

            this.UpdateAppViewBackButtonVisibility();

            var frameworkElement = (FrameworkElement)e.Content;
            this._eventAggregator.PublishOnCurrentThread(new NavigatedEvent(frameworkElement.DataContext));
        }

        private void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

    }
}
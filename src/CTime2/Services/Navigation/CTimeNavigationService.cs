using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;

namespace CTime2.Services.Navigation
{
    public class CTimeNavigationService : FrameAdapter, ICTimeNavigationService
    {
        public CTimeNavigationService(Frame frame, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
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
        }

        private void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

    }
}
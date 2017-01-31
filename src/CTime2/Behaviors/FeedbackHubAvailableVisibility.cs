using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Services.Store.Engagement;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class FeedbackHubAvailableVisibility : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Visibility = StoreServicesFeedbackLauncher.IsSupported()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}

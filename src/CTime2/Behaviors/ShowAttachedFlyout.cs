using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class ShowAttachedFlyout : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Click += this.AssociatedObjectOnClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.Click -= this.AssociatedObjectOnClick;
        }

        private void AssociatedObjectOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            FlyoutBase.ShowAttachedFlyout(this.AssociatedObject);
        }
    }
}
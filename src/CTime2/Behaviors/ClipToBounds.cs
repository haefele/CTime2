using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class ClipToBounds : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SizeChanged += this.AssociatedObjectOnSizeChanged;

            this.UpdateClip();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.SizeChanged -= this.AssociatedObjectOnSizeChanged;
        }

        private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            this.UpdateClip();
        }

        private void UpdateClip()
        {
            this.AssociatedObject.Clip = new RectangleGeometry
            {
                Rect = new Rect(0, 0, this.AssociatedObject.ActualWidth, this.AssociatedObject.ActualHeight)
            };
        }
    }
}
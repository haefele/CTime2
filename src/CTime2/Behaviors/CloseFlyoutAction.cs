using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class CloseFlyoutAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty ControlProperty = DependencyProperty.Register(
            nameof(Control), 
            typeof(object), 
            typeof(CloseFlyoutAction), 
            new PropertyMetadata(default(object)));

        public object Control
        {
            get { return (object)this.GetValue(ControlProperty); }
            set { this.SetValue(ControlProperty, value); }
        }

        object IAction.Execute(object sender, object parameter)
        {
            var element = (this.Control ?? sender) as FrameworkElement;
            if (element == null)
                return null;

            var flyoutPresenter = element.FindVisualAscendant<FlyoutPresenter>();
            if (flyoutPresenter == null)
                return null;

            var popup = flyoutPresenter.Parent as Popup;
            if (popup == null)
                return null;

            popup.IsOpen = false;

            return null;
        }
    }
}
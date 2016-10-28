using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Xaml.Interactivity;

namespace CTime2.Behaviors
{
    public class CloseFlyoutAction : DependencyObject, IAction
    {
        object IAction.Execute(object sender, object parameter)
        {
            var element = sender as FrameworkElement;
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
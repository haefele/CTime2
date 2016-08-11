using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CTime2.Common
{
    public static class VisualTreeHelperEx
    {
        public static T GetParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            while (parent != null)
            {
                if (parent is T)
                    return (T) parent;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return default(T);
        }
    }
}
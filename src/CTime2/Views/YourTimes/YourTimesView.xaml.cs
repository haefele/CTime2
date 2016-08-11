using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using CTime2.Common;

namespace CTime2.Views.YourTimes
{
    public sealed partial class YourTimesView : Page
    {
        public YourTimesViewModel ViewModel => this.DataContext as YourTimesViewModel;

        public YourTimesView()
        {
            this.InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.UpdateBackground((FrameworkElement)sender);
        }

        private void FrameworkElement_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.UpdateBackground(sender);
        }

        private void UpdateBackground(FrameworkElement frameworkElement)
        {
            var headerItem = VisualTreeHelperEx.GetParent<ListViewHeaderItem>(frameworkElement);

            if (headerItem == null)
                return;

            var viewModel = (TimesByDay) frameworkElement.DataContext;

            if (viewModel.Day.DayOfWeek == DayOfWeek.Saturday)
            {
                headerItem.Background = (Brush)Application.Current.Resources["CTimeLightGray"];
            }
            else if (viewModel.Day.DayOfWeek == DayOfWeek.Sunday)
            {
                headerItem.Background = (Brush)Application.Current.Resources["CTimeDarkGray"];
            }
            else
            {
                headerItem.ClearValue(BackgroundProperty);
            }
        }
    }
}

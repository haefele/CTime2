using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using CTime2.Common;
using Microsoft.Toolkit.Uwp.UI;

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
            var headerItem = frameworkElement.FindVisualAscendant<ListViewHeaderItem>();

            if (headerItem == null)
                return;

            var viewModel = (TimesByDay) frameworkElement.DataContext;

            if (viewModel.Day.DayOfWeek == DayOfWeek.Saturday)
            {
                headerItem.Style = (Style)this.Resources["SaturdayListViewHeaderItemStyle"];
            }
            else if (viewModel.Day.DayOfWeek == DayOfWeek.Sunday)
            {
                headerItem.Style = (Style)this.Resources["SundayListViewHeaderItemStyle"];
            }
            else
            {
                headerItem.Style = (Style)this.Resources["DefaultListViewHeaderItemStyle"];
            }
        }
    }
}

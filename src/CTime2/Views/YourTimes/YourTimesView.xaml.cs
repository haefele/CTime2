using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using CTime2.Common;
using CTime2.Core.Data;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Extensions;

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
            var headerItem = frameworkElement.FindAscendant<ListViewHeaderItem>();

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

        private async void WarnMissingDaysSymbolIcon_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var symbolIcon = (SymbolIcon) sender;
            var day = (TimesByDay) symbolIcon.DataContext;

            this.ViewModel.SelectedDayForReportMissingTime = day;
            await this.ViewModel.ReportMissingTimeForSelectedDay.ExecuteAsync();
        }
    }
}

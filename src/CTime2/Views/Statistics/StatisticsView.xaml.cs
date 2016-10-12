using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UwCore.Extensions;

namespace CTime2.Views.Statistics
{
    public sealed partial class StatisticsView : Page
    {
        public StatisticsViewModel ViewModel => this.DataContext as StatisticsViewModel;

        public StatisticsView()
        { 
            this.InitializeComponent();
        }

        private async void CurrentMonth_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StartDate = DateTimeOffset.Now.StartOfMonth();
            this.ViewModel.EndDate = DateTimeOffset.Now.EndOfMonth();

            await this.ViewModel.LoadStatistics.ExecuteAsync();
        }

        private async void LastMonth_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StartDate = DateTimeOffset.Now.StartOfMonth().AddMonths(-1);
            this.ViewModel.EndDate = DateTimeOffset.Now.EndOfMonth().AddMonths(-1);

            await this.ViewModel.LoadStatistics.ExecuteAsync();
        }

        private async void LastSevenDays_OnClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.StartDate = DateTimeOffset.Now.WithoutTime().AddDays(-6); //Last 6 days plus today
            this.ViewModel.EndDate = DateTimeOffset.Now.WithoutTime();

            await this.ViewModel.LoadStatistics.ExecuteAsync();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var statisticItem = (StatisticItem) e.ClickedItem;
            statisticItem.ShowDetails?.Invoke();
        }
    }
}

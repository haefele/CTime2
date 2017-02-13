using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using CTime2.Strings;

namespace CTime2.Views.Statistics.DetailedStatistic
{
    public sealed partial class BreakTime : Page
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public BreakTime()
        {
            this.InitializeComponent();
        }
    }
}

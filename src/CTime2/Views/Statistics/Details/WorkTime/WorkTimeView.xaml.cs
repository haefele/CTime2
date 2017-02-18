using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Telerik.UI.Xaml.Controls.Chart;

namespace CTime2.Views.Statistics.Details.WorkTime
{
    public sealed partial class WorkTimeView : Page
    {
        public WorkTimeViewModel ViewModel => this.DataContext as WorkTimeViewModel;

        public WorkTimeView()
        {
            this.InitializeComponent();
        }
    }
}

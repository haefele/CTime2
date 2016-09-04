using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace CTime2.Views.Statistics.DetailedStatistic
{
    public sealed partial class DisplayDataPoint : UserControl
    {
        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register(
            nameof(ActualValue), typeof(double), typeof(DisplayDataPoint), new PropertyMetadata(default(double), (s, e) => ((DisplayDataPoint)s).OnActualValueChanged((double)e.OldValue, (double)e.NewValue)));

        public double ActualValue
        {
            get { return (double) GetValue(ActualValueProperty); }
            set { SetValue(ActualValueProperty, value); }
        }

        public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register(
            nameof(DisplayValue), typeof(string), typeof(DisplayDataPoint), new PropertyMetadata(default(string)));

        public string DisplayValue
        {
            get { return (string) GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        public DisplayDataPoint()
        {
            this.InitializeComponent();

            this.Loaded += (_, __) => this.OnActualValueChanged(this.ActualValue, this.ActualValue);
        }
        
        private void OnActualValueChanged(double oldValue, double newValue)
        {
            var formatter = this.GetAncestors().OfType<ICustomDataPointFormat>().FirstOrDefault();

            if (formatter == null && this.Tag is DependencyObject)
            {
                formatter = ((DependencyObject) this.Tag).GetAncestors().OfType<ICustomDataPointFormat>().FirstOrDefault();
            }

            this.DisplayValue = formatter != null
                ? formatter.Format(newValue)
                : newValue.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2
{
    partial class Styles : ResourceDictionary
    {
        public Styles()
        {
            this.InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var placementTarget = ToolTipService.GetPlacementTarget((DependencyObject) sender);
        }
    }
}

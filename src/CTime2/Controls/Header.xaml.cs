using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CTime2.Controls
{
    public sealed partial class Header : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Header), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty NavigationButtonVisibilityProperty = DependencyProperty.Register(
            "NavigationButtonVisibility", typeof (Visibility), typeof (Header), new PropertyMetadata(default(Visibility)));

        public Visibility NavigationButtonVisibility
        {
            get { return (Visibility)GetValue(NavigationButtonVisibilityProperty); }
            set { SetValue(NavigationButtonVisibilityProperty, value); }
        }

        public event EventHandler<RoutedEventArgs> NavigationButtonClick;

        public Header()
        {
            this.InitializeComponent();
        }

        private void NavigationButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationButtonClick?.Invoke(this, e);
        }
    }
}
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CTime2.Views.Shell
{
    public sealed partial class ShellView : Page
    {
        public ShellView()
        {
            this.InitializeComponent();

            this.ContentFrame.ContentTransitions = new TransitionCollection
            {
                new NavigationThemeTransition
                {
                    DefaultNavigationTransitionInfo = new EntranceNavigationTransitionInfo()
                }
            };
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (NavigationItemViewModel)e.ClickedItem;
            clickedItem.Execute();

            this.Navigation.IsPaneOpen = false;
        }

        private void OpenNavigationView(object sender, RoutedEventArgs e)
        {
            this.Navigation.IsPaneOpen = true;
        }
    }
}

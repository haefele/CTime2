using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

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

        public ShellViewModel ViewModel => this.DataContext as ShellViewModel;

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (NavigationItemViewModel)e.ClickedItem;
            clickedItem.Execute();

            if (this.WindowSize.CurrentState?.Name == "Narrow")
            {
                this.Navigation.IsPaneOpen = false;
            }
        }

        private void OpenNavigationView(object sender, RoutedEventArgs e)
        {
            this.Navigation.IsPaneOpen = true;
        }
    }
}

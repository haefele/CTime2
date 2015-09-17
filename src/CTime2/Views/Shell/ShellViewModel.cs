using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace CTime2.Views.Shell
{
    public class ShellViewModel : Screen
    {
        public BindableCollection<NavigationItemViewModel> Actions { get; }
        public BindableCollection<NavigationItemViewModel> SecondaryActions { get; }

        public ShellViewModel()
        {
            this.Actions = new BindableCollection<NavigationItemViewModel>();
            this.SecondaryActions = new BindableCollection<NavigationItemViewModel>();

            this.Actions.Add(new NavigationItemViewModel(this.Login) { Label = "Login", Symbol = Symbol.Add });
        }

        private void Login()
        {
            throw new System.NotImplementedException();
        }
    }
}
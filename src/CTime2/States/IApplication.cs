using Caliburn.Micro;
using CTime2.Views.Shell;

namespace CTime2.States
{
    public interface IApplication
    {
        ApplicationState CurrentState { get; set; }

        BindableCollection<NavigationItemViewModel> Actions { get; } 
        BindableCollection<NavigationItemViewModel> SecondaryActions { get; } 
    }
}
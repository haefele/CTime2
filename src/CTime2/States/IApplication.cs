using Caliburn.Micro;
using CTime2.Views.Shell;

namespace CTime2.States
{
    public interface IApplication
    {
        ApplicationState CurrentState { get; set; }

        BindableCollection<HamburgerItem> Actions { get; } 
        BindableCollection<HamburgerItem> SecondaryActions { get; } 
    }
}
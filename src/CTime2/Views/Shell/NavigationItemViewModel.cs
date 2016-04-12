using System;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Action = System.Action;

namespace CTime2.Views.Shell
{
    public class NavigationItemViewModel : PropertyChangedBase
    {
        public NavigationItemViewModel(Action execute, string label, Symbol symbol, Type selectedForViewModelType = null)
        {
            this.Execute = execute;
            this.Label = label;
            this.Symbol = symbol;
            this.SelectedForViewModelType = selectedForViewModelType;
        }

        public Action Execute { get; }
        public Symbol Symbol { get; }
        public Type SelectedForViewModelType { get; }
        public string Label { get; }
    }
}
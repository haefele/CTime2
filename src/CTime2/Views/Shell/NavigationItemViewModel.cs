using Caliburn.Micro;
using Action = System.Action;

namespace CTime2.Views.Shell
{
    public class NavigationItemViewModel : PropertyChangedBase
    {
        public NavigationItemViewModel(Action execute, string label, object symbol)
        {
            this.Execute = execute;
            this.Label = label;
            this.Symbol = symbol;
        }

        public Action Execute { get; }
        public object Symbol { get; }
        public string Label { get; }
    }
}
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using CTime2.Extensions;
using Action = System.Action;

namespace CTime2.Views.Shell
{
    public class NavigationItemViewModel : PropertyChangedBase
    {
        private string _label;
        private object _symbol;

        public NavigationItemViewModel(Action execute)
        {
            this.Execute = execute;
        }

        #region Properties
        public Action Execute { get; }
        public object Symbol
        {
            get { return this._symbol; }
            set { this.SetProperty(ref this._symbol, value); }
        }
        public string Label
        {
            get { return this._label; }
            set { this.SetProperty(ref this._label, value); }
        }

        #endregion
    }
}
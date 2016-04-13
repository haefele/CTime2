using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Shell
{
    public abstract class HamburgerItem
    {
        public HamburgerItem(string label, Symbol symbol)
        {
            this.Label = label;
            this.Symbol = symbol;
        }
        
        public Symbol Symbol { get; }
        public string Label { get; }
    }
}
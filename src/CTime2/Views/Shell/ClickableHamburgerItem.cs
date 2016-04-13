using System;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Shell
{
    public class ClickableHamburgerItem : HamburgerItem
    {
        public ClickableHamburgerItem(string label, Symbol symbol, Action action)
            : base(label, symbol)
        {
            this.Action = action;
        }

        public Action Action { get; }
    }
}
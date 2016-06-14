using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CTime2.Controls
{
    public class StampButton : ContentControl
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(StampButton), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(
            "Symbol", typeof(Symbol), typeof(StampButton), new PropertyMetadata(default(Symbol)));

        public Symbol Symbol
        {
            get { return (Symbol) GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        

        public StampButton()
        {
            this.DefaultStyleKey = typeof(StampButton);
        }
    }
}
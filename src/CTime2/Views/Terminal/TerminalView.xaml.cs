using System;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Views.Terminal
{
    public sealed partial class TerminalView : Page
    {
        private readonly Timer _focusTextBoxTimer;

        public TerminalViewModel ViewModel => this.DataContext as TerminalViewModel;

        public TerminalView()
        {
            this.InitializeComponent();

            this._focusTextBoxTimer = new Timer(this.TryFocusTextBox, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));
        }

        private void TryFocusTextBox(object state)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.RfidReaderTextBox.Focus(FocusState.Programmatic);

            }).AsTask().Wait();
        }
    }
}

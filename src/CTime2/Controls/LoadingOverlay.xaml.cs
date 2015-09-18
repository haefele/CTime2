using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Controls
{
    public partial class LoadingOverlay
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(LoadingOverlay), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(LoadingOverlay), new PropertyMetadata(default(bool)));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public LoadingOverlay()
        {
            this.InitializeComponent();
        }
    }
}

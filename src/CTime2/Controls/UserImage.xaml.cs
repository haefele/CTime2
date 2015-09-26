using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Controls
{
    public sealed partial class UserImage : UserControl
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof (byte[]), typeof (UserImage), new PropertyMetadata(default(byte[])));

        public byte[] Image
        {
            get { return (byte[])GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public UserImage()
        {
            this.InitializeComponent();
        }
    }
}

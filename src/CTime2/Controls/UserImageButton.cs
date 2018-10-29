using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CTime2.Controls
{
    public class UserImageButton : Button
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(byte[]), typeof(UserImageButton), new PropertyMetadata(default(byte[])));

        public byte[] Image
        {
            get { return (byte[])this.GetValue(ImageProperty); }
            set { this.SetValue(ImageProperty, value); }
        }

        public UserImageButton()
        {
            this.DefaultStyleKey = typeof(UserImageButton);
        }
    }
}
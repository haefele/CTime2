using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CTime2.Extensions
{
    public static class ByteArrayExtensions
    {
        public static async Task<ImageSource> ToImage(this byte[] self)
        {
            if (self == null)
                return null;

            if (self.Length == 0)
                return null;

            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(self.AsBuffer());
            stream.Seek(0);

            var myImage = new BitmapImage();
            await myImage.SetSourceAsync(stream);

            return myImage;
        }
    }
}
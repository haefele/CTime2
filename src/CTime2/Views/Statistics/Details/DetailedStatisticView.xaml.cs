using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CTime2.Views.Statistics.Details
{
    public sealed partial class DetailedStatisticView : Page, IDetailedStatisticView
    {
        public DetailedStatisticViewModel ViewModel => this.DataContext as DetailedStatisticViewModel;

        public DetailedStatisticView()
        {
            this.InitializeComponent();
        }

        public async Task<RandomAccessStreamReference> GetDiagramAsync()
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(this.Content);
            
            var randomAccessStream = new InMemoryRandomAccessStream();

            var pixels = await bitmap.GetPixelsAsync();
            var displayInfo = DisplayInformation.GetForCurrentView();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8, 
                BitmapAlphaMode.Premultiplied, 
                (uint)bitmap.PixelWidth, 
                (uint)bitmap.PixelHeight, 
                displayInfo.RawDpiX, 
                displayInfo.RawDpiX,
                pixels.ToArray());
            await encoder.FlushAsync();

            return RandomAccessStreamReference.CreateFromStream(randomAccessStream);
        }
    }

    public interface IDetailedStatisticView
    {
        Task<RandomAccessStreamReference> GetDiagramAsync();
    }
}

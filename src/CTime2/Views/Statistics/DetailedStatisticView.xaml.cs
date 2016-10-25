using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace CTime2.Views.Statistics
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
            
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("StatisticDiagram.png", CreationCollisionOption.ReplaceExisting);
            using (var randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
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
            }
            
            return RandomAccessStreamReference.CreateFromFile(file);
        }
    }

    public interface IDetailedStatisticView
    {
        Task<RandomAccessStreamReference> GetDiagramAsync();
    }
}

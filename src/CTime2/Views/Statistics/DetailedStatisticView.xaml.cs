using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using UwCore.Extensions;

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

            var pixels = await bitmap.GetPixelsAsync();
            var randomAccessStream = await pixels.AsStream().ToRandomAccessStreamAsync();
            var reference = RandomAccessStreamReference.CreateFromStream(randomAccessStream);

            return reference;
        }
    }

    public interface IDetailedStatisticView
    {
        Task<RandomAccessStreamReference> GetDiagramAsync();
    }
}

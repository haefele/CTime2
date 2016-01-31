using System;
using System.Threading.Tasks;

namespace CTime2.Core.Services.Band
{
    public interface IBandService
    {
        Task RegisterBandTileAsync();
        Task StartListeningForEvents();

        Task ShowMessage(string title, string message);

        event EventHandler CheckInPressed;
        event EventHandler CheckOutPressed;
    }
}
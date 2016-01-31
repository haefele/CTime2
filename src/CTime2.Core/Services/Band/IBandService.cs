using System;
using System.Threading.Tasks;

namespace CTime2.Core.Services.Band
{
    public interface IBandService
    {
        Task RegisterBandTileAsync();
        Task StartListeningForEvents();
    }
}
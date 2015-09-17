using System.Threading.Tasks;
using CTime2.Services.CTime;

namespace CTime2.Services.SessionState
{
    public interface ISessionStateService
    {
        User CurrentUser { get; set; }

        Task SaveStateAsync();
        Task RestoreStateAsync();
    }
}
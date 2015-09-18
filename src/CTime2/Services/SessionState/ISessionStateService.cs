using System.Threading.Tasks;
using CTime2.Services.CTime;

namespace CTime2.Services.SessionState
{
    public interface ISessionStateService
    {
        string CompanyId { get; set; }
        User CurrentUser { get; set; }

        Task SaveStateAsync();
        Task RestoreStateAsync();
    }
}
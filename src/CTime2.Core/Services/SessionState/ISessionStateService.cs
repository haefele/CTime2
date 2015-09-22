using System.Threading.Tasks;
using CTime2.Core.Data;
using CTime2.Core.Services.CTime;

namespace CTime2.Core.Services.SessionState
{
    public interface ISessionStateService
    {
        string CompanyId { get; set; }
        User CurrentUser { get; set; }

        Task SaveStateAsync();
        Task RestoreStateAsync();
    }
}
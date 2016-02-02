using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.CTime
{
    public interface ICTimeStampHelperCallback
    {
        Task OnNotLoggedIn();

        bool SupportsQuestions();

        Task OnDidNothing();

        Task<bool> OnAlreadyCheckedInWannaCheckOut();
        Task OnAlreadyCheckedIn();

        Task<bool> OnAlreadyCheckedOutWannaCheckIn();
        Task OnAlreadyCheckedOut();

        Task OnSuccess(TimeState timeState);
    }
}
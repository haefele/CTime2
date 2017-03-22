using System.Threading.Tasks;

namespace CTime2.Core.Services.EmployeeNotification
{
    public interface IEmployeeNotificationService
    {
        bool ShouldNotify(string attendingUserId);
        void Add(string attendingUserId);
        void Remove(string attendingUserId);

        Task SendNotificationsAsync();
    }
}
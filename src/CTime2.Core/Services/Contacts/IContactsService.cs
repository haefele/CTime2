using System.Threading.Tasks;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Contacts
{
    public interface IContactsService
    {
        Task CreateContactAsync(AttendingUser contact);
    }
}
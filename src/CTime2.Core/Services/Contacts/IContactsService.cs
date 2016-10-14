using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Contacts
{
    public interface IContactsService
    {
        Task CreateContactAsync(AttendingUser contact);
    }

    public class ContactsService : IContactsService
    {
        private const string ContactListDisplayName = "c-time";

        public async Task CreateContactAsync(AttendingUser contact)
        {
            var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            var contactLists = await contactStore.FindContactListsAsync();

            var contactList = contactLists.FirstOrDefault(f => f.DisplayName == ContactListDisplayName) ??
                              await contactStore.CreateContactListAsync(ContactListDisplayName);

            var theContact = new Contact
            {
                FirstName = contact.FirstName,
                LastName = contact.Name,
                Phones =
                {
                    new ContactPhone
                    {
                        Kind = ContactPhoneKind.Company,
                        Number = contact.PhoneNumber
                    }
                },
                Emails =
                {
                    new ContactEmail
                    {
                        Kind = ContactEmailKind.Work,
                        Address = contact.EmailAddress
                    }
                }
            };
            await contactList.SaveContactAsync(theContact);
        }
    }
}
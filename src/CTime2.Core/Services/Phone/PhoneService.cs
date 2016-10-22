using Windows.ApplicationModel.Calls;
using Windows.Foundation.Metadata;
using UwCore.Common;

namespace CTime2.Core.Services.Phone
{
    public class PhoneService : IPhoneService
    {
        public bool CanCall => ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1);

        public void Call(string phoneNumber, string displayName)
        {
            Guard.NotNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            Guard.NotNullOrWhiteSpace(displayName, nameof(displayName));

            if (this.CanCall == false)
                return;

            PhoneCallManager.ShowPhoneCallUI(phoneNumber, displayName);
        }
    }
}
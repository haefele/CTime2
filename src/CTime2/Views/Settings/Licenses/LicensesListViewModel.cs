using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.Licenses;
using CTime2.Services.Navigation;
using CTime2.Strings;

namespace CTime2.Views.Settings.Licenses
{
    public class LicensesListViewModel : Screen
    {
        private readonly ICTimeNavigationService _navigationService;
        
        public BindableCollection<License> Licenses { get; }

        public LicensesListViewModel(ILicensesService licensesService, ICTimeNavigationService navigationService)
        {
            this._navigationService = navigationService;

            this.DisplayName = CTime2Resources.Get("Navigation.Licenses");

            this.Licenses = new BindableCollection<License>(licensesService.GetLicenses());
        }
        
        public void ShowLicense(License license)
        {
        }
    }
}

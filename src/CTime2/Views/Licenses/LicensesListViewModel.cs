using Caliburn.Micro;
using CTime2.Core.Data;
using CTime2.Core.Services.Licenses;

namespace CTime2.Views.Licenses
{
    public class LicensesListViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        
        public BindableCollection<License> Licenses { get; }

        public LicensesListViewModel(ILicensesService licensesService, INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this.DisplayName = "Lizenzen";

            this.Licenses = new BindableCollection<License>(licensesService.GetLicenses());
        }
        
        public void ShowLicense(License license)
        {
            this._navigationService
                .For<LicenseViewModel>()
                .WithParam(f => f.DisplayName, license.Name)
                .Navigate();
        }
    }
}

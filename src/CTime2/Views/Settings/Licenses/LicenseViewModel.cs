using System.Linq;
using Caliburn.Micro;
using CTime2.Core.Services.Licenses;
using CTime2.Extensions;
using UwCore.Extensions;

namespace CTime2.Views.Settings.Licenses
{
    public class LicenseViewModel : Screen
    {
        private readonly ILicensesService _licensesService;

        private string _licenseText;

        public string LicenseText
        {
            get { return this._licenseText; }
            set { this.SetProperty(ref this._licenseText, value); }
        }
        
        public LicenseViewModel(ILicensesService licensesService)
        {
            this._licensesService = licensesService;
        }

        protected override void OnActivate()
        {
            var license = this._licensesService.GetLicenses().First(f => f.Name == this.DisplayName);
            this.LicenseText = this._licensesService.GetHtmlLicenseStringFor(license);
        }
    }
}

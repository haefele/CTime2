using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Services.Navigation;
using CTime2.Strings;
using CTime2.Views.Licenses;

namespace CTime2.Views.About
{
    public class AboutViewModel : Screen
    {
        private readonly ICTimeNavigationService _navigationService;

        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.SetProperty(ref this._currentVersion, value); }
        }

        public AboutViewModel(ICTimeNavigationService navigationService)
        {
            this._navigationService = navigationService;
            this.DisplayName = CTime2Resources.Get("Navigation.About");

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();
        }
        
        public void ShowUsedSoftware()
        {
            this._navigationService
                .For<LicensesListViewModel>()
                .Navigate();
        }
    }
}
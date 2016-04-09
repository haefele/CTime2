using System;
using Windows.ApplicationModel;
using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Services.Navigation;
using CTime2.Strings;

namespace CTime2.Views.Settings.About
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
    }
}
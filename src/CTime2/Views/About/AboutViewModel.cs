using System;
using Windows.ApplicationModel;
using Caliburn.Micro;
using CTime2.Extensions;

namespace CTime2.Views.About
{
    public class AboutViewModel : Screen
    {
        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.SetProperty(ref this._currentVersion, value); }
        }

        public AboutViewModel()
        {
            this.DisplayName = "Über";

            this.CurrentVersion = Package.Current.Id.Version.ToVersion();
        }
        
        public void ShowUsedSoftware()
        {
        }
    }
}
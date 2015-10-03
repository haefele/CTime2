﻿using System;
using Windows.ApplicationModel;
using Caliburn.Micro;
using CTime2.Extensions;
using CTime2.Views.Licenses;

namespace CTime2.Views.About
{
    public class AboutViewModel : Screen
    {
        private readonly INavigationService _navigationService;

        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.SetProperty(ref this._currentVersion, value); }
        }

        public AboutViewModel(INavigationService navigationService)
        {
            this._navigationService = navigationService;
            this.DisplayName = "Über";

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
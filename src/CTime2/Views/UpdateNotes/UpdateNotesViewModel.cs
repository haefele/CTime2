using System;
using Windows.ApplicationModel;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore.Extensions;

namespace CTime2.Views.UpdateNotes
{
    public class UpdateNotesViewModel : ReactiveScreen
    {
        private Version _currentVersion;

        public Version CurrentVersion
        {
            get { return this._currentVersion; }
            set { this.RaiseAndSetIfChanged(ref this._currentVersion, value); }
        }

        public UpdateNotesViewModel()
        {
            this.CurrentVersion = Package.Current.Id.Version.ToVersion();
        }
    }
}
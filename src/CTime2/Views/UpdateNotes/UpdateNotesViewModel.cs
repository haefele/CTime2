using Windows.ApplicationModel;
using Caliburn.Micro.ReactiveUI;
using CTime2.Strings;
using ReactiveUI;
using UwCore.Extensions;

namespace CTime2.Views.UpdateNotes
{
    public class UpdateNotesViewModel : ReactiveScreen
    {
        private string _newVersionInstalled;

        public string NewVersionInstalled
        {
            get { return this._newVersionInstalled; }
            set { this.RaiseAndSetIfChanged(ref this._newVersionInstalled, value); }
        }

        public UpdateNotesViewModel()
        {
            this.NewVersionInstalled = CTime2Resources.GetFormatted("TheNewVersionFormat", Package.Current.Id.Version.ToVersion());
        }
    }
}
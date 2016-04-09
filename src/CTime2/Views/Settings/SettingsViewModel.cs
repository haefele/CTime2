using Caliburn.Micro;
using CTime2.Strings;
using CTime2.Views.Settings.About;
using CTime2.Views.Settings.Band;
using CTime2.Views.Settings.Licenses;
using CTime2.Views.Settings.Start;

namespace CTime2.Views.Settings
{
    public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
    {
        public SettingsViewModel()
        {
            this.DisplayName = CTime2Resources.Get("Navigation.Settings");

            this.Items.Add(IoC.Get<AboutViewModel>());
            this.Items.Add(IoC.Get<BandViewModel>());
            this.Items.Add(IoC.Get<LicensesListViewModel>());
            this.Items.Add(IoC.Get<StartViewModel>());
        }
    }
}
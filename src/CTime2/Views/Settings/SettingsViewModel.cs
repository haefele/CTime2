using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using CTime2.Strings;
using CTime2.Views.About;
using CTime2.Views.Settings.Band;
using CTime2.Views.Settings.Others;

namespace CTime2.Views.Settings
{
    public class SettingsViewModel : ReactiveConductor<ReactiveScreen>.Collection.OneActive
    {
        public SettingsViewModel()
        {
            this.DisplayName = CTime2Resources.Get("Navigation.Settings");
            
            this.Items.Add(IoC.Get<BandViewModel>());
            this.Items.Add(IoC.Get<OthersViewModel>());
        }
    }
}
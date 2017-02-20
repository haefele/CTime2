using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CTime2.Views.Settings
{
    public sealed partial class SettingsView : Page
    {
        public SettingsViewModel ViewModel => this.DataContext as SettingsViewModel;

        public SettingsView()
        {
            this.InitializeComponent();

            this.MondayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Monday, this.MondayCheckBox.IsChecked ?? false));
            this.TuesdayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Tuesday, this.TuesdayCheckBox.IsChecked ?? false));
            this.WednesdayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Wednesday, this.WednesdayCheckBox.IsChecked ?? false));
            this.ThursdayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Thursday, this.ThursdayCheckBox.IsChecked ?? false));
            this.FridayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Friday, this.FridayCheckBox.IsChecked ?? false));
            this.SaturdayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Saturday, this.SaturdayCheckBox.IsChecked ?? false));
            this.SundayCheckBox.RegisterPropertyChangedCallback(ToggleButton.IsCheckedProperty, (s, e) => this.WorkDayChanged(DayOfWeek.Sunday, this.SundayCheckBox.IsChecked ?? false));
        }

        private void WorkDayChanged(DayOfWeek day, bool included)
        {
            if (included == false)
            {
                this.ViewModel.WorkDays.Remove(day);
            }
            else if (this.ViewModel.WorkDays.Contains(day) == false)
            {
                this.ViewModel.WorkDays.Add(day);
            }
        }
        
        private void SettingsView_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            this.SyncWorkDaysToView();

            this.ViewModel.WorkDays.Changed.Subscribe(_ =>
            {
                this.SyncWorkDaysToView();
            });
        }

        private void SyncWorkDaysToView()
        {
            this.MondayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Monday);
            this.TuesdayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Tuesday);
            this.WednesdayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Wednesday);
            this.ThursdayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Thursday);
            this.FridayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Friday);
            this.SaturdayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Saturday);
            this.SundayCheckBox.IsChecked = this.ViewModel.WorkDays.Contains(DayOfWeek.Sunday);
        }
    }
}

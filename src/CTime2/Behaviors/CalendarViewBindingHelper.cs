using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using UwCore.Extensions;

namespace CTime2.Behaviors
{
    public class CalendarViewBindingHelper : Behavior<CalendarView>
    {
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
            nameof(SelectedDate), 
            typeof(DateTimeOffset), 
            typeof(CalendarViewBindingHelper), 
            new PropertyMetadata(default(DateTimeOffset), OnSelectedDateChanged));

        public DateTimeOffset SelectedDate
        {
            get { return (DateTimeOffset) this.GetValue(SelectedDateProperty); }
            set { this.SetValue(SelectedDateProperty, value); }
        }

        private bool _firstTime = true;
        private static async void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (CalendarViewBindingHelper)sender;

            self.SynchronizeToCalendarView();

            if (self._firstTime == false)
            {
                //Uuuuh, that code sucks, and I don't like it
                //But we need it, to give the bindings on the SelectedDate the time to update
                //Without it, the SelectedDateChanged event would be fired, before properties, that are bound to the SelectedDate property, are updated
                await Task.Delay(TimeSpan.FromMilliseconds(10)); 
                self.SelectedDateChanged?.Invoke(self, EventArgs.Empty);
            }
            self._firstTime = false;
        }

        public event EventHandler SelectedDateChanged;

        protected override void OnAttached()
        {
            this.AssociatedObject.SelectedDatesChanged += this.AssociatedObjectOnSelectedDatesChanged;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectedDatesChanged -= this.AssociatedObjectOnSelectedDatesChanged;
        }
        
        private void AssociatedObjectOnSelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            this.SynchronizeFromCalendarView();
        }

        private bool _alreadySynchronizing;

        private void SynchronizeToCalendarView()
        {
            if (this._alreadySynchronizing)
                return;

            this._alreadySynchronizing = true;

            this.AssociatedObject.SelectedDates.Clear();
            this.AssociatedObject.SelectedDates.Add(this.SelectedDate);

            this._alreadySynchronizing = false;
        }

        private void SynchronizeFromCalendarView()
        {
            if (this._alreadySynchronizing)
                return;

            this._alreadySynchronizing = true;

            //Only sync when the user has at least one date selected
            //If he unselects the last date he has selected, we will ignore it
            if (this.AssociatedObject.SelectedDates.Any())
            {
                this.SelectedDate = this.AssociatedObject.SelectedDates.First().WithoutTime();
            }

            this._alreadySynchronizing = false;
        }
    }
}
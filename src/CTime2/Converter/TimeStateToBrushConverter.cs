using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using CTime2.Core.Data;

namespace CTime2.Converter
{
    public class TimeStateToBrushConverter : IValueConverter
    {
        public Brush EnteredBrush { get; set; }
        public Brush LeftBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeState == false)
                return value;

            var timeState = (TimeState) value;

            if (timeState.HasFlag(TimeState.Entered))
                return this.EnteredBrush;

            if (timeState.HasFlag(TimeState.Left))
                return this.LeftBrush;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
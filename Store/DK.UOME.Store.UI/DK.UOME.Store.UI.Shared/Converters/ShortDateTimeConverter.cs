using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace DK.UOME.Store.UI.Converters
{
    public class ShortDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTime date = (DateTime)value;

                return string.Format("{0}/{1}/{2}", date.Month, date.Day, date.Year);
            }
            catch (InvalidCastException exception)
            {
                Debug.Assert(false, string.Format("Invalid date time cast: {0}", exception.Message));
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return DateTime.Parse(value.ToString());
            }
            catch (InvalidCastException exception)
            {
                Debug.Assert(false, string.Format("Invalid date time string: {0}", value));
                return DateTime.MinValue;
            }
        }
    }
}

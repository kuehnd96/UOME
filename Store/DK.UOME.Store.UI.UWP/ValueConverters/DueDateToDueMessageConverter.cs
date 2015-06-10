using System;
using Windows.UI.Xaml.Data;

namespace DK.UOME.Store.UI.UWP.ValueConverters
{
    /// <summary>
    ///  Converts a nullable due date to a user-friendly message.
    /// </summary>
    public sealed class DueDateToDueMessageConverter : IValueConverter
    {
        const string NoDateMessage = "No due date";
        const string OverdueMessage = "Overdue";
        const int DayMessageMax = 6;
        const string DayMessageFormat = "Due in {0} days";
        const string DateFormat = "ddd MMM d yyyy";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return NoDateMessage;
            }

            if (value is DateTime)
            {
                DateTime today = DateTime.Now.Date;
                DateTime dueDate = (DateTime)value;

                if (dueDate < today)
                {
                    return OverdueMessage;
                }

                TimeSpan timeUntilDue = dueDate.Date - today;

                if (timeUntilDue <= TimeSpan.FromDays(DayMessageMax))
                {
                    return string.Format(DayMessageFormat, timeUntilDue.Days);
                }
                else
                {
                    return string.Format("Due {0}", dueDate.ToString(DateFormat));
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}

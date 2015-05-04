using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DK.UOME.Store.UI.DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DK.UOME.Store.UI.Converters
{
    public class EntryTypeToVisibilityConverter : IValueConverter
    {
        static readonly Type _visibilityType = typeof(Visibility);
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == _visibilityType)
            {
                if ((value != null) && (parameter != null))
                {
                    EntryType type = (EntryType)value;
                    EntryType desiredType;

                    if (Enum.TryParse<EntryType>(parameter.ToString(), out desiredType))
                    {
                        if (type != desiredType)
                        {
                            return Visibility.Collapsed;
                        }
                    }
                }

                return Visibility.Visible;
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

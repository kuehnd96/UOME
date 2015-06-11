using DK.UOME.Store.UI.DataModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DK.UOME.Store.UI.UWP.Selectors
{
    /// <summary>
    /// Provides logic for selecting the proper data template for the entry type.
    /// </summary>
    public class EntryTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var entry = item as Entry;

            if (entry != null)
            {
                switch (entry.Type)
                {
                    case EntryType.Loaned:
                        return (DataTemplate)App.Current.Resources["LoanedEntryTemplate"];
                    case EntryType.Borrowed:
                        return (DataTemplate)App.Current.Resources["BorrowedEntryTemplate"];
                    default:
                        return null;
                }
            }

            return null;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}

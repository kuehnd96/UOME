using DK.Framework.Store.Model;
using System.Collections.ObjectModel;

namespace DK.UOME.Store.UI.DataModel
{
    /// <summary>
    /// A group of entries.
    /// </summary>
    public class EntryGroup : Group<Entry>
    {
        ObservableCollection<LandingItem<Entry>> _landingItems;

        /// <summary>
        /// Gets or sets a collection of entries and group items.
        /// </summary>
        public ObservableCollection<LandingItem<Entry>> LandingItems
        {
            get { return _landingItems; }
            set { SetProperty(ref _landingItems, value); }
        }

        /// <summary>
        /// Gets or sets the type of entries contained within (if applicable).
        /// </summary>
        public EntryType? Type { get; set; }
    }
}

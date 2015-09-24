using DK.UOME.Store.UI.DataModel.UWP;
using System.Composition;

namespace DK.UOME.Store.PresentationModel.UWP.ViewModels
{
    /// <summary>
    /// Logic for a screen that shows a group of entries.
    /// </summary>
    [Export(typeof(EntryGroupViewModel))]
    [Shared]
    public class EntryGroupViewModel : ShowEntryViewModel
    {
        EntryGroup _entryGroup;

        /// <summary>
        /// Gets or sets the group of entries to show.
        /// </summary>
        public EntryGroup EntryGroup
        {
            get { return _entryGroup; }
            set { SetProperty(ref _entryGroup, value); }
        }
    }
}


namespace DK.UOME.Web.UI.DataModel
{
    /// <summary>
    /// Entry that represents something borrowed.
    /// </summary>
    public class BorrowedEntry : Entry
    {
        /// <summary>
        /// Gets the type of this entry.
        /// </summary>
        public override EntryType Type
        {
            get { return EntryType.Borrowed; }
        }

        /// <summary>
        /// Gets the description of the borroweed item.
        /// </summary>
        public override string ThingLabel
        {
            get { return "Borrowed Item"; }
        }

        /// <summary>
        /// Gets the label for the loaner associated with this entry.
        /// </summary>
        public override string OtherPartyLabel
        {
            get { return "Loaner"; }
        }
    }
}

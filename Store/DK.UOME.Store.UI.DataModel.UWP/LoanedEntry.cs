
namespace DK.UOME.Store.UI.DataModel.UWP
{
    /// <summary>
    /// Entry that represents something loaned.
    /// </summary>
    public class LoanedEntry : Entry
    {
        /// <summary>
        /// Gets the type of this entry.
        /// </summary>
        public override EntryType Type
        {
            get { return EntryType.Loaned; }
        }

        /// <summary>
        /// Gets the description of the loaned item.
        /// </summary>
        public override string ThingLabel
        {
            get { return "Loaned Item"; }
        }

        /// <summary>
        /// Gets the label for the borrower associated with this entry.
        /// </summary>
        public override string OtherPartyLabel
        {
            get { return "Borrower"; }
        }
    }
}

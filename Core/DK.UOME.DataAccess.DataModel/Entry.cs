using System;

namespace DK.UOME.DataAccess.DataModel
{
    /// <summary>
    /// The state of an entry.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Gets or sets the identifier of the entry.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets what item is associated with this entry.
        /// </summary>
        public string Thing { get; set; }
        
        /// <summary>
        /// Gets or sets a descriptive note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date this entry was created.
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// Gets or sets the date this entry must be satisfied by.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the other party involved with this entry.
        /// </summary>
        public string OtherParty { get; set; }
        
        /// <summary>
        /// Gets or sets the type of this entry
        /// </summary>
        /// <value>0 for loaned, 1 for borrowed.</value>
        public int Type { get; set; }

        /// <summary>
        /// Gets the label for this entry's type.
        /// </summary>
        public string TypeLabel
        {
            get
            {
                switch (Type)
                {
                    case 0:
                        return "Loaned";

                    case 1:
                        return "Borrowed";

                    default:
                        return null;
                }
            }
        }
    }
}

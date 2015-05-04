using System;
using System.ComponentModel.DataAnnotations;

namespace DK.UOME.Web.UI.DataModel
{
    /// <summary>
    /// Basic entry state.
    /// </summary>
    public abstract class Entry
    {
        /// <summary>
        /// Gets or sets the identifier of this entry.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the thing associated with this entry.
        /// </summary>
        [Required]
        public string Thing { get; set; }

        /// <summary>
        /// Gets or sets the note associated with this entry.
        /// </summary>
        [StringLength(150)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the creation date of this entry.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the date the entry is due.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the other party associated with this entry.
        /// </summary>
        [Required]
        public string OtherParty { get; set; }

        /// <summary>
        /// Gets the type of this entry.
        /// </summary>
        public abstract EntryType Type { get; }

        /// <summary>
        /// Gets the label for the thing associated with this entry.
        /// </summary>
        public abstract string ThingLabel { get; }

        /// <summary>
        /// Gets the label for the other party associated with this entry.
        /// </summary>
        public abstract string OtherPartyLabel { get; }
    }
}

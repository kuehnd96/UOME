using DK.UOME.DataAccess.DataModel;
using SQLite;
using System;

namespace DK.UOME.Store.DataAccess.Local.UWP
{
    /// <summary>
    /// Represents an entry table in SQLite.
    /// </summary>
    [Table("Entry")]
    public class EntryEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the entry.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets what item is associated with this entry.
        /// </summary>
        [NotNull]
        [MaxLength(30)]
        public string Thing { get; set; }

        /// <summary>
        /// Gets or sets a descriptive note.
        /// </summary>
        [MaxLength(100)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date this entry was created.
        /// </summary>
        [NotNull]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the date this entry must be satisfied by.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the other party involved with this entry.
        /// </summary>
        [NotNull]
        [MaxLength(30)]
        public string OtherParty { get; set; }

        /// <summary>
        /// Gets or sets the type of this entry
        /// </summary>
        /// <value>0 for loaned, 1 for borrowed.</value>
        public int Type { get; set; }

        /// <summary>
        /// Explicit cast of an entity to the form of an entry <see cref="Entry">data model object.</see>
        /// </summary>
        /// <param name="entity">The entry entity to make a copy of.</param>
        /// <returns></returns>
        public static explicit operator Entry(EntryEntity entity)
        {
            Entry entry = new Entry();

            entry.CreateDate = entity.CreateDate;
            entry.DueDate = entity.DueDate;
            entry.Id = entity.Id;
            entry.Note = entity.Note;
            entry.OtherParty = entity.OtherParty;
            entry.Thing = entity.Thing;
            entry.Type = entity.Type;

            return entry;
        }
    }
}

using SQLite;

namespace DK.UOME.Store.DataAccess.Local
{
    /// <summary>
    /// Represents a table of pinned entries in SQLite.
    /// </summary>
    [Table("PinnedEntry")]
    public class PinnedEntry
    {
        /// <summary>
        /// Gets or sets the identifier of the pinned entry.
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }
    }
}

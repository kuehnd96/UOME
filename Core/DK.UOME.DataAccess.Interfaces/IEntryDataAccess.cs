using DK.UOME.DataAccess.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.UOME.DataAccess.Interfaces
{
    /// <summary>
    /// Persisted storage of entry data.
    /// </summary>
    public interface IEntryDataAccess
    {
        /// <summary>
        /// Updates an existing entry.
        /// </summary>
        /// <param name="updatedEntry">The new state of the entry.</param>
        /// <returns>A task enabling async execution.</returns>
        Task UpdateEntry(Entry updatedEntry);

        /// <summary>
        /// Adds an entry to persisted storage.
        /// </summary>
        /// <param name="newEntry">The entry to add to storage.</param>
        /// <returns>The identifier of the new entry.</returns>
        Task<int> AddEntry(Entry newEntry);

        /// <summary>
        /// Removes an entry from storage.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove.</param>
        /// <returns>A task enabling async execution.</returns>
        Task DeleteEntry(int obsoleteEntryId);

        /// <summary>
        /// Determines if an entry exists in storage.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        Task<bool> EntryExists(int entryId);

        /// <summary>
        /// Provides all entries in storage.
        /// </summary>
        /// <returns>A collection of entries in storage.</returns>
        Task<IList<Entry>> LoadEntries();

        /// <summary>
        /// Loads the identifiers of all pinned entries.
        /// </summary>
        /// <returns>List of entry identifiers.</returns>
        Task<IList<int>> GetAllPinnedEntryIds();

        /// <summary>
        /// Removes an entry identifier from the collection of pinned entries.
        /// </summary>
        /// <param name="entryId">The identifier of the entry that was unpinned.</param>
        /// <returns></returns>
        Task UnpinEntry(int unpinnedEntryId);

        /// <summary>
        /// Adds an entry identifier to the collection of pinned entries.
        /// </summary>
        /// <param name="entryId">The identifier of the entry that was pinned.</param>
        /// <returns></returns>
        Task PinEntry(int pinnedEntryId);

        /// <summary>
        /// Loads a single entry by identifier.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to load.</param>
        /// <returns></returns>
        Task<Entry> GetEntryById(int entryId);
    }
}

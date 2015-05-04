using DK.UOME.DataAccess.DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DK.UOME.Repositories.Interfaces
{
    /// <summary>
    /// Access entry data.
    /// </summary>
    public interface IEntryRepository
    {
        /// <summary>
        /// Saves entry data.
        /// </summary>
        /// <param name="updatedEntry">The entry state to save.</param>
        /// <returns>A task that enables async execution.</returns>
        Task SaveEntry(Entry updatedEntry);

        /// <summary>
        /// Removes an entry.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove.</param>
        /// <returns>A task that enables async execution.</returns>
        Task DeleteEntry(int obsoleteEntryId);

        /// <summary>
        /// Removes multiple entries.
        /// </summary>
        /// <param name="obsoleteEntryIds">A collection of identifier of entries to be deleted.</param>
        /// <returns>A task that enables async execution.</returns>
        Task DeleteEntries(IEnumerable<int> obsoleteEntryIds);

        /// <summary>
        /// Determines whether a entry exists.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        Task<bool> EntryExists(int entryId);

        /// <summary>
        /// Gets a collection of actions to perform when entry data changes.
        /// </summary>
        List<Func<Task>> UpdateActions { get; }

        /// <summary>
        /// Loads all entries.
        /// </summary>
        /// <returns>A collection of all entries.</returns>
        Task<IList<Entry>> GetAllEntries();

        /// <summary>
        /// Loads the identifiers of all pinned entries.
        /// </summary>
        /// <returns>List of entry identifiers.</returns>
        Task<IList<int>> GetAllPinnedEntrtIds();

        /// <summary>
        /// Removes an entry identifier from the collection of pinned entries.
        /// </summary>
        /// <param name="entryId">The identifier of the entry that was unpinned.</param>
        /// <returns></returns>
        Task UnpinEntry(int entryId);

        /// <summary>
        /// Adds an entry identifier to the collection of pinned entries.
        /// </summary>
        /// <param name="entryId">The identifier of the entry that was pinned.</param>
        /// <returns></returns>
        Task PinEntry(int entryId);

        /// <summary>
        /// Loads a single entry by identifier.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to load.</param>
        /// <returns></returns>
        Task<Entry> GetEntryById(int entryId);
    }
}

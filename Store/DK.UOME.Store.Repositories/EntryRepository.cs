using DK.Framework.Core;
using DK.UOME.DataAccess.DataModel;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;

namespace DK.UOME.Store.Repositories
{
    /// <summary>
    /// Access entry data.
    /// </summary>
    [Export(typeof(IEntryRepository))]
    [Shared]
    public class EntryRepository : IEntryRepository
    {
        /// <summary>
        /// Gets or sets the data access instance to access data.
        /// </summary>
        /// <remarks>MEF imported.</remarks>
        //[Import("Cloud")]
        [Import("Local")]
        public IEntryDataAccess EntryDataAccess { get; set; }

        /// <summary>
        /// Gets a collection of actions to perform when entry data changes.
        /// </summary>
        public List<Func<Task>> UpdateActions { get; private set; }

        /// <summary>
        /// Creates an instance with no update actions.
        /// </summary>
        public EntryRepository()
        {
            UpdateActions = new List<Func<Task>>(1);
        }

        /// <summary>
        /// Saves entry data.
        /// </summary>
        /// <param name="updatedEntry">The entry state to save.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task SaveEntry(Entry updatedEntry)
        {
            Requires.IsNotNull(updatedEntry, "Updated entry is null.");

            var entryExists = await EntryDataAccess.EntryExists(updatedEntry.Id);

            if (entryExists)
            {
                await EntryDataAccess.UpdateEntry(updatedEntry);
            }
            else
            {
                updatedEntry.Id = await EntryDataAccess.AddEntry(updatedEntry);
            }

            RunUpdateActions();
        }

        /// <summary>
        /// Removes an entry.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task DeleteEntry(int obsoleteEntryId)
        {
            Requires.IsTrue(obsoleteEntryId > 0, "Obsolete entry Id is less than zero.");

            var entryExists = await EntryDataAccess.EntryExists(obsoleteEntryId);

            if (entryExists)
            {
                await EntryDataAccess.DeleteEntry(obsoleteEntryId);

                RunUpdateActions();
            }
        }

        /// <summary>
        /// Removes multiple entries.
        /// </summary>
        /// <param name="obsoleteEntryIds">A collection of identifier of entries to be deleted.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task DeleteEntries(IEnumerable<int> obsoleteEntryIds)
        {
            Requires.IsNotNull(obsoleteEntryIds, "Collection of obsolete entry ids is null.");

            bool entryExists;
            bool hasDeletionOccurred = false;

            foreach (var obsoleteEntryId in obsoleteEntryIds)
            {
                entryExists = await EntryDataAccess.EntryExists(obsoleteEntryId);

                if (entryExists)
                {
                    await EntryDataAccess.DeleteEntry(obsoleteEntryId);

                    hasDeletionOccurred = true;
                }
            }

            if (hasDeletionOccurred)
            {
                RunUpdateActions();
            }
        }

        /// <summary>
        /// Determines whether a entry exists.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        public async Task<bool> EntryExists(int entryId)
        {
            Requires.IsTrue(entryId > 0, "Entry Id is less than zero.");

            return await EntryDataAccess.EntryExists(entryId);
        }

        /// <summary>
        /// Loads all entries.
        /// </summary>
        /// <returns>A collection of all entries.</returns>
        public async Task<IList<Entry>> GetAllEntries()
        {
            return await EntryDataAccess.LoadEntries();
        }

        void RunUpdateActions()
        {
            foreach (var updateAction in UpdateActions)
            {
                updateAction();
            }
        }

        public async Task<IList<int>> GetAllPinnedEntrtIds()
        {
            return await EntryDataAccess.GetAllPinnedEntryIds();
        }

        public async Task UnpinEntry(int entryId)
        {
            await EntryDataAccess.UnpinEntry(entryId);
        }

        public async Task PinEntry(int entryId)
        {
            await EntryDataAccess.PinEntry(entryId);
        }

        public async Task<Entry> GetEntryById(int entryId)
        {
            return await EntryDataAccess.GetEntryById(entryId);
        }
    }
}

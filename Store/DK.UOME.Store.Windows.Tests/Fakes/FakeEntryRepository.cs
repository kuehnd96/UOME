using DK.UOME.DataAccess.DataModel;
using DK.UOME.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.Store.Windows.Tests.Fakes
{
    [Export(typeof(IEntryRepository))]
    internal sealed class FakeEntryRepository : IEntryRepository
    {
        readonly List<Entry> _entries;
        readonly List<int> _pinnedEntries;

        /// <summary>
        /// Gets a collection of actions to perform when entry data changes.
        /// </summary>
        public List<Func<Task>> UpdateActions { get; private set; }

        /// <summary>
        /// Creates an instance with dummy data for testing.
        /// </summary>
        public FakeEntryRepository()
        {
            DateTime now = DateTime.Now.Date;
            
            _entries = new List<Entry>(5);
            _entries.Add(new Entry() { CreateDate = now.AddDays(-23), DueDate = now.AddDays(-2), Id = 1, Note = "This is overdue", OtherParty = "Eric Elser", Thing = "Super Mario Brothers Wii", Type = 1 });
            _entries.Add(new Entry() { CreateDate = now.AddDays(-23), DueDate = now.AddDays(1), Id = 2, Note = "Xbox One disc game", OtherParty = "Rob Sheehy", Thing = "Rayman Origins", Type = 0 });
            _entries.Add(new Entry() { CreateDate = now.AddDays(-10), Id = 3, Note = "Covred me at lunch", OtherParty = "Bob Zurad", Thing = "$13", Type = 1 });
            _entries.Add(new Entry() { CreateDate = now.AddDays(-6), Id = 4, OtherParty = "Mom", Thing = "Xmen DVD's", Type = 0 });
            _entries.Add(new Entry() { CreateDate = now, Id = 5, OtherParty = "Jon Kuehn", Thing = "3 Blu Ray discs", Type = 1 });

            _pinnedEntries = new List<int>(5);

            UpdateActions = new List<Func<Task>>();
        }

        /// <summary>
        /// Saves entry data.
        /// </summary>
        /// <param name="updatedEntry">The entry state to save.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task SaveEntry(UOME.DataAccess.DataModel.Entry updatedEntry)
        {
            var entryToUpdate = _entries.Single(entry => entry.Id == updatedEntry.Id);

            entryToUpdate.DueDate = updatedEntry.DueDate;
            entryToUpdate.Note = updatedEntry.Note;
            entryToUpdate.OtherParty = updatedEntry.OtherParty;
            entryToUpdate.Thing = updatedEntry.Thing;

            RunUpdateActions();
        }

        /// <summary>
        /// Removes an entry.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task DeleteEntry(int obsoleteEntryId)
        {
            var obsoleteEntry = _entries.Single(entry => entry.Id == obsoleteEntryId);

            _entries.Remove(obsoleteEntry);

            RunUpdateActions();
        }

        /// <summary>
        /// Removes multiple entries.
        /// </summary>
        /// <param name="obsoleteEntryIds">A collection of identifier of entries to be deleted.</param>
        /// <returns>A task that enables async execution.</returns>
        public async Task DeleteEntries(IEnumerable<int> obsoleteEntryIds)
        {
            foreach (var obsoleteEntryId in obsoleteEntryIds)
            {
                var obsoleteEntry = _entries.Single(entry => entry.Id == obsoleteEntryId);

                _entries.Remove(obsoleteEntry);
            }

            RunUpdateActions();
        }

        /// <summary>
        /// Determines whether a entry exists.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        public async Task<bool> EntryExists(int entryId)
        {
            return _entries.Any(entry => entry.Id == entryId);
        }

        /// <summary>
        /// Loads all entries.
        /// </summary>
        /// <returns>A collection of all entries.</returns>
        public async Task<IList<UOME.DataAccess.DataModel.Entry>> GetAllEntries()
        {
            return new List<Entry>(_entries);
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
            return new List<int>(_pinnedEntries);
        }

        public async Task UnpinEntry(int entryId)
        {
            _pinnedEntries.Remove(entryId);
        }

        public async Task PinEntry(int entryId)
        {
            _pinnedEntries.Add(entryId);
        }

        public async Task<Entry> GetEntryById(int entryId)
        {
            return _entries.Where(entry => entry.Id == entryId).FirstOrDefault();
        }
    }
}

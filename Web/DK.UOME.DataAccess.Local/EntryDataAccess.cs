using DK.UOME.DataAccess.DataModel;
using DK.UOME.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.DataAccess.Local
{
    [Export(typeof(IEntryDataAccess))]
    public class EntryDataAccess : IEntryDataAccess
    {
        List<Entry> _entries = new List<Entry>();
        //TODO: Update web local data access with pinned entry functionalty.

        public EntryDataAccess()
        {
            _entries.Add(new Entry { CreateDate = DateTime.Today.AddDays(-7), DueDate = DateTime.Today.AddDays(7), Id = 1, OtherParty = "Eric Elser", Thing = "Super Mario Brothers Wii", Type = 1, Note = "Wii disc game." });
            _entries.Add(new Entry { CreateDate = new DateTime(2014, 4, 3), Id = 2, OtherParty = "Bob Zurad", Thing = "Money for resturant apps", Type = 1, Note = "He bought the apps at Carlson's" });
            _entries.Add(new Entry { CreateDate = DateTime.Today, Id = 3, OtherParty = "Damon Payne", Thing = "Kinect Joyride", Type = 0, Note = "Xbox 360 disc game." });
            _entries.Add(new Entry { CreateDate = DateTime.Today.AddDays(-14), DueDate = DateTime.Today.AddDays(-3), Id = 4, OtherParty = "Mike Schulz", Thing = "$23", Type = 1, Note = "Spotted me for lunch two weeks ago." });
            _entries.Add(new Entry { CreateDate = DateTime.Today.AddDays(-14), DueDate = DateTime.Today.AddDays(30), Id = 5, OtherParty = "Andy Leichtle", Thing = "$40", Type = 0, Note = "Money for team dinner." });
        }
        
        /// <summary>
        /// Updates an existing entry.
        /// </summary>
        /// <param name="updatedEntry">The new state of the entry.</param>
        /// <returns>A task enabling async execution.</returns>
        public Task UpdateEntry(Entry updatedEntry)
        {
            var existingEntry = _entries.Where(entry => entry.Id == updatedEntry.Id).FirstOrDefault();

            if (existingEntry != null)
            {
                return new Task(() =>
                {
                    existingEntry.DueDate = updatedEntry.DueDate;
                    existingEntry.Note = updatedEntry.Note;
                    existingEntry.OtherParty = updatedEntry.OtherParty;
                    existingEntry.Thing = updatedEntry.Thing;
                });
            }

            return new Task(() => existingEntry = null);
        }

        /// <summary>
        /// Adds an entry to persisted storage.
        /// </summary>
        /// <param name="newEntry">The entry to add to storage.</param>
        /// <returns>The identifier of the new entry.</returns>
        public Task<int> AddEntry(Entry newEntry)
        {
            int lastId = _entries.Max(entry => entry.Id);
            
            newEntry.Id = lastId + 1;
            return new Task<int>(() =>
            {
                _entries.Add(newEntry);
                return newEntry.Id;
            });
        }

        /// <summary>
        /// Removes an entry from storage.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove.</param>
        /// <returns>A task enabling async execution.</returns>
        public Task DeleteEntry(int obsoleteEntryId)
        {
            var obsoleteEntry = _entries.Where(entry => entry.Id == obsoleteEntryId).FirstOrDefault();

            if (obsoleteEntry != null)
            {
                return new Task(() => _entries.Remove(obsoleteEntry));
            }

            return new Task(() => obsoleteEntry = null);
        }

        /// <summary>
        /// Determines if an entry exists in storage.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        public Task<bool> EntryExists(int entryId)
        {
            return new Task<bool>(() => { return _entries.Any(entry => entry.Id == entryId); });
        }

        /// <summary>
        /// Provides all entries in storage.
        /// </summary>
        /// <returns>A collection of entries in storage.</returns>
        public async Task<IList<Entry>> LoadEntries()
        {
            return new List<Entry>(_entries);
        }


        public async Task<IList<int>> GetAllPinnedEntryIds()
        {
            if (_entries.Any())
            {
                return new List<int>(_entries.First().Id);
            }

            return new List<int>();
            
        }

        public async Task UnpinEntry(int unpinnedEntryId)
        {
            // Left blank for now
        }

        public async Task PinEntry(int pinnedEntryId)
        {
            // Left blank for now
        }

        public async Task<Entry> GetEntryById(int entryId)
        {
            // Left blank for now
            return new Entry();
        }
    }
}

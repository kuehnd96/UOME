using DK.Framework.Core;
using DK.UOME.DataAccess.DataModel;
using DK.UOME.DataAccess.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using CoreWindows = Windows;

namespace DK.UOME.Store.Windows.DataAccess.Local
{
    /// <summary>
    /// Persisted storage of entry data from a local SQLite database.
    /// </summary>
    [Export("Local", typeof(IEntryDataAccess))]
    [Shared]
    public class EntryDataAccess : IEntryDataAccess
    {
        static readonly string _connectionString = @"\UOME.db";

        async Task<SQLiteAsyncConnection> EnsureDatabase()
        {
            var path = CoreWindows.Storage.ApplicationData.Current.LocalFolder.Path + _connectionString;
            var database = new SQLiteAsyncConnection(path);

            // Create entry table if it doesn't exist
            await database.CreateTableAsync<EntryEntity>();

            return database;
        }

        /// <summary>
        /// Updates an existing entry.
        /// </summary>
        /// <param name="updatedEntry">The new state of the entry. Cannot be null.</param>
        /// <returns>A task enabling async execution.</returns>
        public async Task UpdateEntry(Entry updatedEntry)
        {
            Requires.IsNotNull(updatedEntry, "Updated entry cannot be null.");

            var database = await EnsureDatabase();
            var dirtyEntry = await database.FindAsync<EntryEntity>(updatedEntry.Id);

            // Update entry
            dirtyEntry.Thing = updatedEntry.Thing;
            dirtyEntry.Note = updatedEntry.Note;
            dirtyEntry.DueDate = updatedEntry.DueDate;
            dirtyEntry.OtherParty = updatedEntry.OtherParty;

            await database.UpdateAsync(dirtyEntry);
        }

        /// <summary>
        /// Removes an entry from storage.
        /// </summary>
        /// <param name="obsoleteEntryId">The identifier of the entry to remove. Must be more than zero.</param>
        /// <returns>A task enabling async execution.</returns>
        public async Task DeleteEntry(int obsoleteEntryId)
        {
            Requires.IsTrue(obsoleteEntryId > 0, "Obsolete entry Id is less than zero.");

            var database = await EnsureDatabase();
            var obsoleteEntry = await database.FindAsync<EntryEntity>(obsoleteEntryId);

            if (obsoleteEntry != null)
            {
                await database.DeleteAsync(obsoleteEntry);
            }
        }

        /// <summary>
        /// Adds an entry to persisted storage.
        /// </summary>
        /// <param name="newEntry">The entry to add to storage.</param>
        /// <returns>The identifier of the new entry.</returns>
        public async Task<int> AddEntry(Entry newEntry)
        {
            Requires.IsNotNull(newEntry, "New entry cannot be null.");

            newEntry.CreateDate = DateTime.Now;

            EntryEntity newEntity = new EntryEntity();
            newEntity.CreateDate = newEntry.CreateDate;
            newEntity.DueDate = newEntry.DueDate;
            newEntity.Note = newEntry.Note;
            newEntity.OtherParty = newEntry.OtherParty;
            newEntity.Thing = newEntry.Thing;
            newEntity.Type = newEntry.Type;

            var database = await EnsureDatabase();

            //Check for add
            await database.InsertAsync(newEntity);

            return newEntity.Id;
        }

        /// <summary>
        /// Determines if an entry exists in storage.
        /// </summary>
        /// <param name="entryId">The identifier of the entry to check for.</param>
        /// <returns>True if the entry exists; Otherwise false.</returns>
        public async Task<bool> EntryExists(int entryId)
        {
            var database = await EnsureDatabase();
            var entry = await database.FindAsync<EntryEntity>(entryId);

            return (null != entry);
        }

        /// <summary>
        /// Provides all entries in storage.
        /// </summary>
        /// <returns>A collection of entries in storage.</returns>
        public async Task<IList<Entry>> LoadEntries()
        {
            var database = await EnsureDatabase();
            var entities = await database.Table<EntryEntity>().ToListAsync();
            var entries = new List<Entry>(entities.Count);

            foreach (var entity in entities)
            {
                Entry entry = (Entry)entity;

                entries.Add(entry);
            }

            return entries;
        }
    }
}

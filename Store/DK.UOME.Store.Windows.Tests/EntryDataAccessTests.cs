using DK.Framework.Core;
using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageModel = DK.UOME.DataAccess.DataModel;

namespace DK.UOME.Store.Tests
{
    [TestClass]
    public sealed class EntryDataAccessTests : TestBase
    {
        readonly string[] _notes = new string[] { "This is the first test note.", "This is test note #2." };
        readonly string[] _otherParties = new string[] { "Jon Kuehn", "Damon Payne", "Rob Sheehy", "Eric Elser" };
        readonly string[] _things = new string[] { "Rayman Legends", "Super Mario Brothers Wii", "$34", "A bunch of Blu Ray discs" };

        [TestMethod]
        public void LocalEntryRepositoryTest()
        {
            IEntryDataAccess dataAccess = Initializer.GetSingleExport<IEntryDataAccess>("Local");
            DateTime now = DateTime.Now.Date;

            var newEntry = new StorageModel.Entry();
            newEntry.CreateDate = now;
            newEntry.DueDate = now.AddDays(7);
            newEntry.Note = _notes[0];
            newEntry.OtherParty = _otherParties[0];
            newEntry.Thing = _things[0];
            newEntry.Type = 0;

            // 1. Test Save new
            var addEntryTask = dataAccess.AddEntry(newEntry);
            Task.WaitAll(new Task[] { addEntryTask });
            newEntry.Id = addEntryTask.Result;

            TestEntry(newEntry, dataAccess, "first added entry", 1, newEntry.Id);

            // 2. Test save update
            newEntry.DueDate = null;
            newEntry.Note = _notes[1];
            newEntry.OtherParty = _otherParties[1];
            newEntry.Thing = _things[1];
            
            var updateTask = dataAccess.UpdateEntry(newEntry);
            Task.WaitAll(new Task[] { updateTask });

            TestEntry(newEntry, dataAccess, "first upated entry", 1, newEntry.Id);

            // 3. Add second entry
            var secondEntry = new StorageModel.Entry();
            secondEntry.CreateDate = now;
            secondEntry.Note = _notes[0];
            secondEntry.OtherParty = _otherParties[2];
            secondEntry.Thing = _things[2];
            secondEntry.Type = 1;

            addEntryTask = dataAccess.AddEntry(secondEntry);
            Task.WaitAll(new Task[] { addEntryTask });
            secondEntry.Id = addEntryTask.Result;

            TestEntry(secondEntry, dataAccess, "second added entry", 2, secondEntry.Id);

            // 4. Update second entry
            secondEntry.Note = _notes[1];
            secondEntry.OtherParty = _otherParties[3];
            secondEntry.Thing = _things[3];

            updateTask = dataAccess.UpdateEntry(secondEntry);
            Task.WaitAll(new Task[] { updateTask });

            TestEntry(secondEntry, dataAccess, "second updated entry", 2, secondEntry.Id);

            // 5. Test delete single
            var deleteTask = dataAccess.DeleteEntry(secondEntry.Id);
            Task.WaitAll(new Task[] { deleteTask });

            // Assert that it doesn't exist
            var existsTask = dataAccess.EntryExists(secondEntry.Id);
            Task.WaitAll(new Task[] { existsTask });
            Assert.IsFalse(existsTask.Result, "Second entry still exists after deletion.");

            var loadAllTask = dataAccess.LoadEntries();
            Task.WaitAll(new Task[] { loadAllTask });
            Assert.AreEqual(loadAllTask.Result.Count, 1, "Expecting only one entry left after deletion.");
        }

        [TestMethod]
        public void LocalPinnedEntryRepositoryTest()
        {
            List<int> entryIds = new List<int>() { 45, 67 };
            IEntryDataAccess dataAccess = Initializer.GetSingleExport<IEntryDataAccess>("Local");
            
            //1. Check for none
            var getPinnedEntiesTask = dataAccess.GetAllPinnedEntryIds();
            Task.WaitAll(new Task[] {getPinnedEntiesTask});
            Assert.IsFalse(getPinnedEntiesTask.Result.Any(), "Pinned entries should have been empty.");

            //2. Add one
            var firstEntry = entryIds.First();
            var addFirstTask = dataAccess.PinEntry(firstEntry);
            Task.WaitAll(new Task[] { addFirstTask });
            
            getPinnedEntiesTask = dataAccess.GetAllPinnedEntryIds();
            Task.WaitAll(new Task[] { getPinnedEntiesTask });
            Assert.AreEqual<int>(1, getPinnedEntiesTask.Result.Count, string.Format("Only one pinned entry should have returned instead of {0}.", getPinnedEntiesTask.Result.Count));
            Assert.IsTrue(getPinnedEntiesTask.Result.Any(entryId => entryId == firstEntry), "First pinned entry was not returned.");

            //3. Add another
            var secondEntry = entryIds.Skip(1).Take(1).First();
            var addSecondTask = dataAccess.PinEntry(secondEntry);
            Task.WaitAll(new Task[] { addSecondTask });

            getPinnedEntiesTask = dataAccess.GetAllPinnedEntryIds();
            Task.WaitAll(new Task[] { getPinnedEntiesTask });
            Assert.AreEqual<int>(2, getPinnedEntiesTask.Result.Count, string.Format("Only two pinned entry should have returned instead of {0}.", getPinnedEntiesTask.Result.Count));
            Assert.IsTrue(getPinnedEntiesTask.Result.Any(entryId => entryId == firstEntry), "First pinned entry was not returned.");
            Assert.IsTrue(getPinnedEntiesTask.Result.Any(entryId => entryId == secondEntry), "Second pinned entry was not returned.");
            
            //4. Delete first
            var deleteFirstTask = dataAccess.UnpinEntry(firstEntry);
            Task.WaitAll(new Task[] { deleteFirstTask });

            getPinnedEntiesTask = dataAccess.GetAllPinnedEntryIds();
            Task.WaitAll(new Task[] { getPinnedEntiesTask });
            Assert.AreEqual<int>(1, getPinnedEntiesTask.Result.Count, string.Format("Only one pinned entry should have returned instead of {0}.", getPinnedEntiesTask.Result.Count));
            Assert.IsTrue(getPinnedEntiesTask.Result.Any(entryId => entryId == secondEntry), "Second pinned entry was not returned.");

            //5. Delete second
            var deleteSecondTask = dataAccess.UnpinEntry(secondEntry);
            Task.WaitAll(new Task[] { deleteSecondTask });

            getPinnedEntiesTask = dataAccess.GetAllPinnedEntryIds();
            Task.WaitAll(new Task[] { getPinnedEntiesTask });
            Assert.IsFalse(getPinnedEntiesTask.Result.Any(), "Pinned entries should have been empty.");
        }

        void TestEntry(StorageModel.Entry entry, IEntryDataAccess dataAccess, string entryLabel, int expectedEntryCount, int entryId)
        {
            // Assert entry exists
            var existsTask = dataAccess.EntryExists(entry.Id);
            Task.WaitAll(new Task[] { existsTask });
            Assert.IsTrue(existsTask.Result, string.Format("Entry does not exist: {0}", entryLabel));

            // Assert new entry exists when loading all entries
            var loadAllTask = dataAccess.LoadEntries();
            Task.WaitAll(new Task[] { loadAllTask });
            Assert.AreEqual(loadAllTask.Result.Count, expectedEntryCount, 
                string.Format("Loading all entries did not return {0} entries: {1}", expectedEntryCount, entryLabel));

            // Assert new entry accuracy
            var getByIdTask = dataAccess.GetEntryById(entryId);
            Task.WaitAll(new Task[] { getByIdTask });

            var loadedEntry = getByIdTask.Result;
            Assert.IsTrue(AreSameSecond(loadedEntry.CreateDate, entry.CreateDate), string.Format("Create date doesn't match: {0}", entryLabel));
            Assert.IsTrue(AreSameSecond(loadedEntry.DueDate, entry.DueDate), string.Format("Due date doesn't match: {0}", entryLabel));
            Assert.AreEqual(loadedEntry.Id, entry.Id, string.Format("Identifier doesn't match: {0}", entryLabel));
            Assert.AreEqual(loadedEntry.Note, entry.Note, string.Format("Note doesn't match {0}", entryLabel));
            Assert.AreEqual(loadedEntry.OtherParty, entry.OtherParty, string.Format("Other party doesn't match {0}", entryLabel));
            Assert.AreEqual(loadedEntry.Thing, entry.Thing, string.Format("Thing doesn't match: {0}", entryLabel));
            Assert.AreEqual(loadedEntry.Type, entry.Type, string.Format("Type doesn't match: {0}", entryLabel));
        }
    }
}

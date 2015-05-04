using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.Repositories;
using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.Windows.DataAccess.Local;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StorageModel = DK.UOME.DataAccess.DataModel;

namespace DK.UOME.Store.Tests.Windows.UnitTests
{
    [TestClass]
    public sealed class EntryDataAccessTests
    {
        readonly DateTime _now = DateTime.Now;
        readonly string[] _notes = new string[] { "This is the first test note.", "This is test note #2." };
        readonly string[] _otherParties = new string[] { "Jon Kuehn", "Damon Payne", "Rob Sheehy", "Eric Elser" };
        readonly string[] _things = new string[] { "Rayman Legends", "Super Mario Brothers Wii", "$34", "A bunch of Blu Ray discs" };
        
        [AssemblyInitialize]
        public static void Startup(TestContext context)
        {
            CompositionStarter starter = new CompositionStarter(null, null);

            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(EntryDataAccessTests).GetTypeInfo().Assembly,
                typeof(DK.UOME.DataAccess.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(EntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.UI.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.Windows.DataAccess.Local.EntryDataAccess).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);
        }
        
        [TestMethod]
        public void Win8LocalEntryRepositoryTest()
        {
            IEntryDataAccess dataAccess = Initializer.GetSingleExport<IEntryDataAccess>("Local");
            
            var newEntry = new StorageModel.Entry();
            newEntry.CreateDate = _now;
            newEntry.DueDate = _now.AddDays(7);
            newEntry.Note = _notes[0];
            newEntry.OtherParty = _otherParties[0];
            newEntry.Thing = _things[0];
            newEntry.Type = 0;

            var addEntryTask = dataAccess.AddEntry(newEntry);
            Task.WaitAll(new Task[] { addEntryTask });
            newEntry.Id = addEntryTask.Result;

            // Assert new entry exists
            var existsTask = dataAccess.EntryExists(newEntry.Id);
            Task.WaitAll(new Task[] { existsTask });
            Assert.IsTrue(existsTask.Result, "Newly added entry does not exist.");
            
            //task = Task.Factory.StartNew(() => result = dataAccess.LoadEntries());
            //Task.Factory.ContinueWhenAll(new Task[] { task }, (tasks) =>
            //    {
            //        IList<StorageModel.Entry> entries = result as IList<StorageModel.Entry>;
            //        Assert.IsTrue(entries.Any(), "Selecting all entries after adding one yielded no results.");
            //        Assert.IsNotNull(entries.Single<StorageModel.Entry>(entry => entry.Id == newEntry.Id), "Newly added entry not returned when getting all entries.");
            //    });
            
            // Test Save new
            // Test save update
            // Test delete single
            // Test delete multiple

            // All the while testing update actions and exists
        }
    }
}

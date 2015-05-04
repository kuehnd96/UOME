using DK.Framework.Store;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using DK.Framework.Core.Interfaces;
using DK.UOME.Store.Windows.Tests.Fakes;

namespace DK.UOME.Store.Windows.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void MainEntryGroupsTest()
        {
            //NOTE: The results of this test is directly tied to the entries created in the constructor of FakeEntryRepository

            const int NumberOfEntryGroups = 4;
            
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();
            var now = DateTime.Now.Date;
            
            var startTask = mainViewModel.Start();
            Task.WaitAll(new Task[] { startTask });

            // 1.  Ensure that entry groups were properly initialized
            Assert.AreEqual<int>(NumberOfEntryGroups, mainViewModel.EntryGroups.Count, string.Format("View model has {0} entry groups instead of the expected {1}.", mainViewModel.EntryGroups.Count, NumberOfEntryGroups));
            
            // Test overdue group
            TestEntryGroup(mainViewModel.EntryGroups[0], 1, new int[] {1});

            // Test next due group
            TestEntryGroup(mainViewModel.EntryGroups[1], 1, new int[] {2});

            // Test borrowed group
            TestEntryGroup(mainViewModel.EntryGroups[2], 3, new int[] { 1, 3, 5 });

            // Test loaned group
            TestEntryGroup(mainViewModel.EntryGroups[3], 2, new int[] { 2, 4});

            //===========================================

            // 2. Test changes to entries
            var getAllEntriesTask = mainViewModel.EntryRepository.GetAllEntries();
            Task.WaitAll(new Task[] { getAllEntriesTask });
            var entries = getAllEntriesTask.Result;
            
            // Edit due date of an entry to make it due in the future
            var entryToUpdate = entries.Single(entry => entry.Id == 4);
            entryToUpdate.DueDate = now.AddDays(4);
            var updateEntryTask = mainViewModel.EntryRepository.SaveEntry(entryToUpdate);
            Task.WaitAll(new Task[] { updateEntryTask });

            // Retest next due group to make sure the updated entry is included
            TestEntryGroup(mainViewModel.EntryGroups[1], 2, new int[] { 2, 4 });
        }

        void TestEntryGroup(EntryGroup group, int expectedCount, int[] entryIds)
        {
            int itemCount = group.Items.Count;
            Assert.AreEqual<int>(expectedCount, itemCount, 
                string.Format(
                "Group '{0}' is expected to have {1} items, instead it has {2}.", 
                group.Name, 
                expectedCount, 
                itemCount));

            foreach (int entryId in entryIds)
            {
                Assert.IsTrue(group.Items.Any(entry => entry.Id == entryId), 
                    string.Format("Entry with identifier {1} not found in group '{1}'.", entryId, group.Name));
            }
        }

        [TestMethod]
        public void MainGroupNavigationTest()
        {
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            var startTask = mainViewModel.Start();
            Task.WaitAll(new Task[] { startTask });

            // 1. Test navigating to the overdue group
            var overdueGroup = mainViewModel.EntryGroups[0];
            mainViewModel.NavigateGroupCommand.Execute(overdueGroup);

            // Ensure that the naviagation target's state is properly updated
            Assert.IsNotNull(navigationTarget.Current, "Entry group navigation didn't navigate to a new page.");
            Assert.IsInstanceOfType(
                navigationTarget.Current,
                typeof(FakeEntryGroupScreen),
                string.Format(
                    "Group navigation didn't navigate to the right page. It navigated to the page: {0}", 
                    navigationTarget.Current.GetType().FullName));

            // Ensure that the new screen has the proper state
            var entryGroupScreen = navigationTarget.Current as IScreen<EntryGroupViewModel>;
            Assert.IsNotNull(entryGroupScreen.ViewModel.EntryGroup, "Entry group navigation did not set the entry group on the screen view model.");
            Assert.AreEqual<string>(overdueGroup.Name, entryGroupScreen.ViewModel.EntryGroup.Name, "Entry group navigation did not set the right entry group on the screen view model.");
        }

        [TestMethod]
        public void MainDeletionPreventionTest()
        {
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();

            // Make sure entry deletion is not allowed
            Assert.IsFalse(mainViewModel.DeleteEntriesCommand.CanExecute(null), "Main view model allows deletion when it shouldn't.");
        }
    }
}

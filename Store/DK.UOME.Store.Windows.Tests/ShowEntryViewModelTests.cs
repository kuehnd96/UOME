using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.Windows.Tests.Fakes;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.Store.Windows.Tests
{
    [TestClass]
    public class ShowEntryViewModelTests
    {
        [TestMethod]
        public void AddNewBorrowedCommandTest()
        {
            AddEntryCommandTest(EntryType.Borrowed, typeof(BorrowedEntry));
        }

        [TestMethod]
        public void AddNewLoanedCommandTest()
        {
            AddEntryCommandTest(EntryType.Loaned, typeof(LoanedEntry));
        }

        void AddEntryCommandTest(EntryType typeofEntry, Type entryType)
        {
            string parameter = typeofEntry.ToString();
            var showEntryViewModel = Initializer.GetSingleExport<MainViewModel>();
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            Assert.IsTrue(showEntryViewModel.AddNewEntryCommand.CanExecute(parameter), "Add entry command cannot execute.");

            // Simulate navigting to add a new borrowed entry
            showEntryViewModel.AddNewEntryCommand.Execute(parameter);

            // Ensure that the naviagation target's state is properly updated
            Assert.IsNotNull(navigationTarget.Current, "Entry group navigation didn't navigate to a new page.");
            Assert.IsInstanceOfType(
                navigationTarget.Current,
                typeof(FakeEntryScreen),
                string.Format(
                    "Adding a new borrowed entry didn't navigate to the right page. It navigated to the page: {0}",
                    navigationTarget.Current.GetType().FullName));

            // Ensure that the new screen has the proper state
            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            Assert.IsTrue(entryScreen.ViewModel.IsNewEntry, "The view model for the entry screen does not indicate a new entry.");
            Assert.IsNotNull(entryScreen.ViewModel.Entry, "The view model for the entry screen does not contain a new blank entry.");
            Assert.IsInstanceOfType(entryScreen.ViewModel.Entry, entryType, string.Format("New entry is not of the correct type. It is of type {0}.", entryScreen.ViewModel.Entry.GetType().FullName));
            Assert.IsFalse(string.IsNullOrEmpty(entryScreen.ViewModel.Label), "The view model for the entry screen does not contain an entry label.");
        }

        [TestMethod]
        public void DeleteCommandCanExecuteTest()
        {
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();
            var entryGroupViewModel = Initializer.GetSingleExport<EntryGroupViewModel>();

            // Assert that you cannot delete until something is selected
            Assert.IsFalse(entryGroupViewModel.DeleteEntriesCommand.CanExecute(new object()), "View model should not allow deleting until entries are selected.");

            // Update view model to feature selected entries
            var startTask = mainViewModel.Start();
            Task.WaitAll(new Task[] { startTask });
            var entries = mainViewModel.EntryGroups[2].Items.Take(2);
            
            foreach (var entry in entries)
            {
                entryGroupViewModel.SelectedEntries.Add(entry);
            }

            // Test deletion functionality
            Assert.IsTrue(entryGroupViewModel.DeleteEntriesCommand.CanExecute(new object()), "View model should allow deleting.");
        }

        [TestMethod]
        public void NavigateEntryTest()
        {
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            var startTask = mainViewModel.Start();
            Task.WaitAll(new Task[] { startTask });
            var entry = mainViewModel.EntryGroups[2].Items.First();

            // Navigate to entry
            mainViewModel.NavigateEntry(entry);

            // Ensure that the naviagation target's state is properly updated
            Assert.IsNotNull(navigationTarget.Current, "Entry navigation didn't navigate to a new page.");
            Assert.IsInstanceOfType(
                navigationTarget.Current,
                typeof(FakeEntryScreen),
                string.Format(
                    "Entry navigation didn't navigate to the right page. It navigated to the page: {0}",
                    navigationTarget.Current.GetType().FullName));

            // Ensure that the new screen has the proper state
            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            Assert.IsNotNull(entryScreen.ViewModel.Entry, "Entry navigation did not set the entry on the screen view model.");
            Assert.AreEqual<int>(entry.Id, entryScreen.ViewModel.Entry.Id, "Entry navigation did not set the right entry on the screen view model.");
        }
    }
}

using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.Store.Windows.Tests
{
    [TestClass]
    public class EntryViewModelTests
    {
        [TestMethod]
        public void EntryEditCancelTest()
        {
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            NavigateToEntryGroup(EntryType.Borrowed);

            var entryGroupScreen = navigationTarget.Current as IScreen<EntryGroupViewModel>;
            var entryGroupViewModel = entryGroupScreen.ViewModel;

            // Navigate to entry edit
            entryGroupViewModel.NavigateEntry(entryGroupViewModel.EntryGroup.Items.First());

            Assert.IsNotNull(navigationTarget.Current, "Navigation to entry screen yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryViewModel>), string.Format("Navigation to entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));

            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            var entryViewModel = entryScreen.ViewModel;

            // Cancel edit
            entryViewModel.CancelCommand.Execute(new object());

            // Assert that navigation went back one screen
            Assert.IsNotNull(navigationTarget.Current, "Cancelling the editing of an entry yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryGroupViewModel>), string.Format("Cancelling the editing of an entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));
        }

        [TestMethod]
        public void ExistingEntryDeleteTest()
        {
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            NavigateToEntryGroup(EntryType.Loaned);

            var entryGroupScreen = navigationTarget.Current as IScreen<EntryGroupViewModel>;
            var entryGroupViewModel = entryGroupScreen.ViewModel;
            int groupEntryCount = entryGroupViewModel.EntryGroup.Items.Count;
            var entryIds = entryGroupViewModel.EntryGroup.Items.Select(entry => entry.Id);

            // Navigate to entry edit
            entryGroupViewModel.NavigateEntry(entryGroupViewModel.EntryGroup.Items.First());

            Assert.IsNotNull(navigationTarget.Current, "Navigation to entry screen yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryViewModel>), string.Format("Navigation to entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));

            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            var entryViewModel = entryScreen.ViewModel;

            Assert.IsTrue(entryViewModel.DeleteCommand.CanExecute(new object()), "Entry view model should be able to delete an existing entry.");
            entryViewModel.DeleteCommand.Execute(new object());

            //NOTE: No need to test if the entry is gone as that is covered in other tests

            // Assert that navigation went back one screen
            Assert.IsNotNull(navigationTarget.Current, "Deleting an entry yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryGroupViewModel>), string.Format("Deleting an entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));
        }

        [TestMethod]
        public void NewEntryDeleteTest()
        {
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            NavigateToEntryGroup(EntryType.Loaned);

            var entryGroupScreen = navigationTarget.Current as IScreen<EntryGroupViewModel>;
            var entryGroupViewModel = entryGroupScreen.ViewModel;

            // Navigate to entry edit
            entryGroupViewModel.AddNewEntryCommand.Execute(EntryType.Loaned.ToString());
                    
            Assert.IsNotNull(navigationTarget.Current, "Navigation to entry screen yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryViewModel>), string.Format("Navigation to entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));

            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            var entryViewModel = entryScreen.ViewModel;

            Assert.IsFalse(entryViewModel.DeleteCommand.CanExecute(new object()), "Entry view model should not be able to delete a new entry.");
        }

        [TestMethod]
        public void EntrySaveTest()
        {
            var navigationTarget = Initializer.GetSingleExport<INavigationTarget>();

            NavigateToEntryGroup(EntryType.Borrowed);

            var entryGroupScreen = navigationTarget.Current as IScreen<EntryGroupViewModel>;
            var entryGroupViewModel = entryGroupScreen.ViewModel;

            // Navigate to entry edit
            entryGroupViewModel.NavigateEntry(entryGroupViewModel.EntryGroup.Items.First());

            Assert.IsNotNull(navigationTarget.Current, "Navigation to entry screen yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryViewModel>), string.Format("Navigation to entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));

            var entryScreen = navigationTarget.Current as IScreen<EntryViewModel>;
            var entryViewModel = entryScreen.ViewModel;

            Assert.IsTrue(entryViewModel.SaveCommand.CanExecute(new object()), "Entry view model should be able to save any entry.");
            entryViewModel.SaveCommand.Execute(new object());

            //NOTE: No need to test if the entry is updated as that is covered in other tests

            // Assert that navigation went back one screen
            Assert.IsNotNull(navigationTarget.Current, "Saving an entry yielded null for a current screen.");
            Assert.IsInstanceOfType(navigationTarget.Current, typeof(IScreen<EntryGroupViewModel>), string.Format("Saving an entry showed the wrong screen: {0}", navigationTarget.Current.GetType().FullName));
        }

        void NavigateToEntryGroup(EntryType groupType)
        {
            var mainViewModel = Initializer.GetSingleExport<MainViewModel>();

            var startTask = mainViewModel.Start();
            Task.WaitAll(new Task[] { startTask });

            EntryGroup group = null;

            switch (groupType)
	        {
		        case EntryType.Loaned:
                    group = mainViewModel.EntryGroups[3];
                    break;

                case EntryType.Borrowed:
                    group = mainViewModel.EntryGroups[2];
                    break;
	        }

            mainViewModel.NavigateGroupCommand.Execute(group);
        }
    }
}

using DK.Framework.Core;
using DK.Framework.Core.Interfaces;
using DK.Framework.Store.Commands;
using DK.Framework.Store.ViewModels;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Popups;

namespace DK.UOME.Store.PresentationModel.ViewModels
{
    /// <summary>
    /// View model for showing entries.
    /// </summary>
    public abstract class ShowEntryViewModel : ScreenViewModel
    {
        static readonly string _borrowedEntryLabel = "Borrowed Entry";
        static readonly string _loanedEntryLabel = "Loaned Entry";
        static readonly string _newBorrowedEntryLabel = "New Borrowed Entry";
        static readonly string _newLoanedEntryLabel = "New Loaned Entry";

        DelegateCommand _addNewEntryCommand;
        DelegateCommand _deleteEntriesCommand;
        IList<Entry> _selectedEntries = new List<Entry>();

        /// <summary>
        /// Repository for stored entries.
        /// </summary>
        [Import]
        public IEntryRepository EntryRepository { get; set; }

        /// <summary>
        /// Invokes functionality for adding a new entry.
        /// </summary>
        public DelegateCommand AddNewEntryCommand
        {
            get
            {
                if (null == _addNewEntryCommand)
                {
                    _addNewEntryCommand = new DelegateCommand(NavigateNewEntry);
                }

                return _addNewEntryCommand;
            }
        }

        /// <summary>
        /// Invokes functionality for deleting entries.
        /// </summary>
        public DelegateCommand DeleteEntriesCommand
        {
            get
            {
                if (null == _deleteEntriesCommand)
                {
                    _deleteEntriesCommand = new DelegateCommand(ConfirmDeleteEntries, CanDeleteEntries);
                }

                return _deleteEntriesCommand;
            }
        }

        /// <summary>
        /// Gets or sets a collection of selected entries.
        /// </summary>
        public IList<Entry> SelectedEntries
        {
            get { return _selectedEntries; }
        }

        void NavigateToEntry(Entry entry, string label)
        {
            Requires.IsNotTrue(string.IsNullOrEmpty(label), "Label prameter is null or empty.");

            Navigation.Navigate<IScreen<EntryViewModel>>(screen =>
            {
                screen.ViewModel.Entry = entry;
                screen.ViewModel.Label = label;
            });
        }

        void ShowNewEntry(EntryType type)
        {
            switch (type)
            {
                case EntryType.Loaned:
                    NavigateToEntry(new LoanedEntry(), _newLoanedEntryLabel);
                    break;

                case EntryType.Borrowed:
                    NavigateToEntry(new BorrowedEntry(), _newBorrowedEntryLabel);
                    break;

                default:
                    Debug.Assert(false, "Unsupported entry type.");
                    break;
            }
        }

        void NavigateNewEntry(object parameter)
        {
            string entryTypeString = parameter as String;

            if (null != entryTypeString)
            {
                EntryType newEntryType;

                if (Enum.TryParse(entryTypeString, out newEntryType))
                {
                    ShowNewEntry(newEntryType);
                }
            }
        }

        /// <summary>
        /// Whether or not deletion of entries is enabled.
        /// </summary>
        /// <param name="parameter">Not used.</param>
        /// <returns>True if deletion is enabled; Otherwise false.</returns>
        protected virtual bool CanDeleteEntries(object parameter)
        {
            return SelectedEntries.Any();
        }

        async void ConfirmDeleteEntries(object parameter)
        {
            var deletionConfirmationDialog = new MessageDialog("Are you sure you want to delete the selected entries?", "Entry Deletion Confirmation");
            deletionConfirmationDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(DeleteSelectedEntries)));
            deletionConfirmationDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(command => { })));
            deletionConfirmationDialog.DefaultCommandIndex = 0;
            deletionConfirmationDialog.CancelCommandIndex = 1;

            await deletionConfirmationDialog.ShowAsync();
        }

        void DeleteSelectedEntries(IUICommand command)
        {
            var obsoleteEntryIds = SelectedEntries.Select(entry => entry.Id);

            EntryRepository.DeleteEntries(obsoleteEntryIds);
        }

        /// <summary>
        /// Navigates to show entry details.
        /// </summary>
        /// <param name="entry">The entry to show. Cannot be null.</param>
        public void NavigateEntry(Entry entry)
        {
            Requires.IsNotNull(entry, "Entry parameter is null.");

            string entryLabel = GetEntryLabel(entry.Type);

            NavigateToEntry(entry, entryLabel);
        }

        /// <summary>
        /// Gets the text label for a type of entry.
        /// </summary>
        /// <param name="entryType">The type of entry to get a label for.</param>
        /// <returns>The label associated with the entry type.</returns>
        public static string GetEntryLabel(EntryType entryType)
        {
            string label = string.Empty;

            switch (entryType)
            {
                case EntryType.Loaned:
                    label = _loanedEntryLabel;
                    break;

                case EntryType.Borrowed:
                    label = _borrowedEntryLabel;
                    break;

                default:
                    Debug.Assert(false, "Unsupported entry type.");
                    break;
            }

            return label;
        }

        /// <summary>
        /// Navigates to a view where the user will see search results based on a search term.
        /// </summary>
        /// <param name="queryText">The search term to search for.</param>
        public void PerformSearch(string queryText)
        {
            Navigation.Navigate<IScreen<SearchViewModel>>(searchResultsScreen =>
            {
                searchResultsScreen.ViewModel.SearchTerm = queryText;
            });
        }
    }
}

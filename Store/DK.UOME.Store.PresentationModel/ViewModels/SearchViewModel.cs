using DK.Framework.Store.Model;
using DK.UOME.Store.PresentationModel.MappingConfigurations.Profiles;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.Store.PresentationModel.ViewModels
{
    /// <summary>
    /// Logic for the search functionality.
    /// </summary>
    [Export(typeof(SearchViewModel))]
    [Shared]
    public class SearchViewModel : ShowEntryViewModel
    {
        const string AllFilterName = "All";
        const string BorrowedFilterName = "Borrowed";
        const string LoanedFilterName = "Loaned";
        
        string _searchTerm;
        bool _hasResults;
        ObservableCollection<SearchFilter<Entry>> _searchFilters;
        SearchFilter<Entry> _selectedFilter;

        /// <summary>
        /// Gets or sets the selected entry filter.
        /// </summary>
        public SearchFilter<Entry> SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != null)
                {
                    _selectedFilter.IsActive = false; 
                }

                value.IsActive = true;
                SetProperty(ref _selectedFilter, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the search term that drives search results.
        /// </summary>
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        /// <summary>
        /// Gets or sets whether or not any entries satisfy the search term.
        /// </summary>
        public bool HasResults
        {
            get { return _hasResults; }
            set { SetProperty(ref _hasResults, value); }
        }

        /// <summary>
        /// Gets or sets the collection of search filters.
        /// </summary>
        public ObservableCollection<SearchFilter<Entry>> Filters
        {
            get { return _searchFilters; }
            set { SetProperty(ref _searchFilters, value); }
        }

        /// <summary>
        /// Populates the search filters.
        /// </summary>
        public SearchViewModel()
        {
            var filters = new List<SearchFilter<Entry>>(3);
            filters.Add(new SearchFilter<Entry>(AllFilterName, true));
            filters.Add(new SearchFilter<Entry>(BorrowedFilterName, false));
            filters.Add(new SearchFilter<Entry>(LoanedFilterName, false));

            Filters = new ObservableCollection<SearchFilter<Entry>>(filters);
            SelectedFilter = Filters[0];
        }

        /// <summary>
        /// Updates search filters with their results.
        /// </summary>
        public async Task Search()
        {
            var searchTerm = SearchTerm.Trim();
            var rawEntries = await EntryRepository.GetAllEntries();
            var allEntries = EntryModelProfile.MapEntryList(rawEntries);

            // Retain entries whose note, other party, or thing contain the supplied text
            var entries = (from entry in allEntries
                           where (((null != entry.Note) &&
                                  (!string.IsNullOrEmpty(entry.Note)) &&
                                 (entry.Note.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)) ||
                                  entry.OtherParty.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  entry.Thing.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                           select entry).ToList();

            Filters[0].Results = new ObservableCollection<Entry>(entries);
            Filters[1].Results = new ObservableCollection<Entry>(entries.Where(entry => entry.Type == EntryType.Borrowed));
            Filters[2].Results = new ObservableCollection<Entry>(entries.Where(entry => entry.Type == EntryType.Loaned));

            if (entries.Count > 0)
            {
                HasResults = true;
            }
            else
            {
                HasResults = false;
            }
        }
    }
}

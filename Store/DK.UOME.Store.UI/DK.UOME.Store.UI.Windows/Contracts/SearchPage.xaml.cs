using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace DK.UOME.Store.UI.Windows.Contracts
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    [Screen(typeof(IScreen<SearchViewModel>))]
    [Shared]
    public sealed partial class SearchPage : BaseStorePage, IScreen<SearchViewModel>
    {
        public SearchPage()
        {
            this.InitializeComponent();

            ViewModel = Initializer.GetSingleExport<SearchViewModel>();
        }

        public SearchViewModel ViewModel
        {
            get { return this.DataContext as SearchViewModel; }
            set { this.DataContext = value; }
        }

        public void End(Action completed)
        {
            completed();
        }

        public Type ScreenType
        {
            get { return typeof(SearchPage); }
        }

        public async void Start(Action completed)
        {
            await ViewModel.Search();
            
            completed();
        }

        public string Title
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Location { get { return "/EntryPage.xaml"; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnItemGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Keep view model in sync with grid selections
            foreach (var item in e.AddedItems.Cast<Entry>())
            {
                ViewModel.SelectedEntries.Add(item);
            }

            foreach (var item in e.RemovedItems.Cast<Entry>())
            {
                ViewModel.SelectedEntries.Remove(item);
            }

            ViewModel.DeleteEntriesCommand.RaiseCanExecuteChanged();
        }

        private void OnEntryItemClicked(object sender, ItemClickEventArgs e)
        {
            var entry = e.ClickedItem as Entry;

            if (null != entry)
            {
                ViewModel.NavigateEntry(entry);
            }
        }

        private async void OnSearchQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            ViewModel.SearchTerm = args.QueryText.Trim();
            await ViewModel.Search();
        }
    }
}

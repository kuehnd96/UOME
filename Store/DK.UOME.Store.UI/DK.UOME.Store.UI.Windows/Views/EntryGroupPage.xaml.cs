using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Composition;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace DK.UOME.Store.UI.Windows.Views
{
    [Screen(typeof(IScreen<EntryGroupViewModel>))]
    [Shared]
    public sealed partial class EntryGroupPage : DK.Framework.Store.BaseStorePage, IScreen<EntryGroupViewModel>
    {
        public EntryGroupPage()
        {
            this.InitializeComponent();

            ViewModel = Initializer.GetSingleExport<EntryGroupViewModel>();
        }

        //FUTURE: No need for this with ancestor binding
        private void OnEntryItemClicked(object sender, ItemClickEventArgs e)
        {
            var entry = e.ClickedItem as Entry;

            if (null != entry)
            {
                
                ViewModel.NavigateEntry(entry);
            }
        }

        public EntryGroupViewModel ViewModel
        {
            get
            {
                return DataContext as EntryGroupViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        public void End(Action completed)
        {
            completed();
        }

        public Type ScreenType
        {
            get { return typeof(EntryGroupPage); }
        }

        public void Start(Action completed)
        {
            completed();
        }

        public string Location { get { return "/EntryPage.xaml"; } }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnItemGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void OnSearchQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            ViewModel.PerformSearch(args.QueryText.Trim());
        }
    }
}

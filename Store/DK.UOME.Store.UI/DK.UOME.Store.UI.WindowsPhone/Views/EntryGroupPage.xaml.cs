using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Composition;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DK.UOME.Store.UI.WindowsPhone.Views
{
    [Screen(typeof(IScreen<EntryGroupViewModel>))]
    [Shared]
    public sealed partial class EntryGroupPage : BaseStorePage, IScreen<EntryGroupViewModel>
    {
        public EntryGroupPage()
        {
            this.InitializeComponent();

            ViewModel = Initializer.GetSingleExport<EntryGroupViewModel>();
        }

        public EntryGroupViewModel ViewModel
        {
            get { return DataContext as EntryGroupViewModel; }
            set { DataContext = value; }
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

        public string Location { get { return "/Views/EntryGroupPage.xaml"; } }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnEntryItemClicked(object sender, ItemClickEventArgs e)
        {
            var entry = e.ClickedItem as Entry;

            if (null != entry)
            {

                ViewModel.NavigateEntry(entry);
            }
        }
    }
}

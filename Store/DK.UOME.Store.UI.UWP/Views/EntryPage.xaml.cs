using DK.Framework.Core.Interfaces;
using DK.Framework.UWP;
using DK.Framework.UWP.Attributes;
using DK.UOME.Store.PresentationModel.UWP.ViewModels;
using DK.UOME.Store.UI.DataModel.UWP;
using DK.UOME.Store.UI.UWP.DesignData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DK.UOME.Store.UI.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Screen(typeof(IScreen<EntryViewModel>))]
    [Shared]
    public sealed partial class EntryPage : BaseStorePage, IScreen<EntryViewModel>
    {
        const string EntryStateKey = "Entry";
        const string LabelStateKey = "Label";

        public event PropertyChangedEventHandler PropertyChanged;

        public EntryViewModel ViewModel
        {
            get { return base.DataContext as EntryViewModel; }
            set { base.DataContext = value; }
        }

        public Type ScreenType
        {
            get { return typeof(EntryPage); }
        }

        public async void Start(Action completed)
        {
            await ViewModel.Start();

            completed();
        }

        public void End(Action completed)
        {
            completed();
        }

        public string Location { get { return "/EntryPage.xaml"; } }

        public EntryPage()
        {
            this.InitializeComponent();

#if DEBUG
            ViewModel = new DesignEntryViewModel();
            ViewModel.Entry.IsTrackingChanges = true;
#endif
        }

        //LIVETILE
        /*async*/
        void OnSecondaryTilePinButtonClick(object sender, RoutedEventArgs e)
        {
            //string secondaryTileId = ViewModel.Entry.Id.ToString();
            //string activationArguments = string.Format("Entry {0} pinned at {1}.", secondaryTileId, DateTime.Now.ToLocalTime().ToString());
            //ISessionState sessionState = Initializer.GetSingleExport<ISessionState>();

            //SecondaryTile secondaryTile = new SecondaryTile(secondaryTileId,
            //                                    "UOME",
            //                                    activationArguments,
            //                                    //TODO: Change this secondary tile logo
            //                                    new Uri("ms-appx:///Assets/square150x150Tile-sdk.png"),
            //                                    TileSize.Square150x150);

            //secondaryTile.RoamingEnabled = false;
            //secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
            //secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            //secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            //secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;

            //await ViewModel.PinEntry();

            //await secondaryTile.RequestCreateAsync();

            //NOTE: Since the user is taken right to the start screen nothing here will run.
        }

        /*async*/ void OnSecondaryTileUnpinButtonClick(object sender, RoutedEventArgs e)
        {
            //SecondaryTile secondaryTile = new SecondaryTile(ViewModel.Entry.Id.ToString());
            //bool wasUnpinned = await secondaryTile.RequestDeleteAsync();

            //if (wasUnpinned)
            //{
            //    await ViewModel.UnpinEntry();
            //}
        }

        private async void OnDeleteAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            EntryViewModel viewModel = this.DataContext as EntryViewModel;

            if (viewModel != null)
            {
                if (viewModel.DeleteCommand.CanExecute(null))
                {
                    MessageDialog deleteConfirmDialog = new MessageDialog("Are you sure you want to delete this entry?", "Delete Entry?");

                    deleteConfirmDialog.Commands.Add(new UICommand("Cancel", (command) => { } ));
                    deleteConfirmDialog.Commands.Add(new UICommand("Delete", (command) => 
                    { 
                        //TODO: Run delete command
                    }));
                    deleteConfirmDialog.DefaultCommandIndex = 1;
                    deleteConfirmDialog.CancelCommandIndex = 0;

                    await deleteConfirmDialog.ShowAsync();
                }
                else
                {
                    MessageDialog cantDeleteDialog = new MessageDialog("You cannot delete this entry because it has not been saved yet.", "Delete Not Allowed");

                    await cantDeleteDialog.ShowAsync();
                }
            }
        }

        private async void OnSaveAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            EntryViewModel viewModel = this.DataContext as EntryViewModel;

            if (viewModel != null)
            {
                // NOTE: Check on HasChanged is in the view model Save() method
                if (viewModel.Entry.IsValid)
                {
                    //TODO: Run save command
                }
                else
                {
                    MessageDialog cantSaveDialog = new MessageDialog("You cannot save this entry until you fix the errors.", "Entry Not Valid");

                    await cantSaveDialog.ShowAsync();
                }
            }
        }

        private async void OnCancelAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            EntryViewModel viewModel = this.DataContext as EntryViewModel;

            if (viewModel != null)
            {
                if (viewModel.Entry.HasChanged)
                {
                    MessageDialog cancelConfirmDialog = new MessageDialog("There are pending changes to your entry. Are you sure you want to discard them?", "Discard Changes?");

                    cancelConfirmDialog.Commands.Add(new UICommand("No", (command) => { }));
                    cancelConfirmDialog.Commands.Add(new UICommand("Yes", (command) =>
                    {
                        //TODO: Run cancel command
                    }));
                    cancelConfirmDialog.DefaultCommandIndex = 1;
                    cancelConfirmDialog.CancelCommandIndex = 0;

                    await cancelConfirmDialog.ShowAsync();
                }
                else
                {
                    viewModel.CancelCommand.Execute(null);
                }
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            pageState[EntryStateKey] = ViewModel.Entry;
            pageState[LabelStateKey] = ViewModel.Label;
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            if (pageState != null)
            {
                ViewModel.Entry = (Entry)pageState[EntryStateKey];
                ViewModel.Label = pageState[LabelStateKey].ToString();
            }
        }
    }
}

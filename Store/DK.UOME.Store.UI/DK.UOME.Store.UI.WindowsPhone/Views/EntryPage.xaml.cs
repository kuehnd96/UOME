using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Composition;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace DK.UOME.Store.UI.WindowsPhone.Views
{
    [Screen(typeof(IScreen<EntryViewModel>))]
    [Shared]
    public sealed partial class EntryPage : BaseStorePage, IScreen<EntryViewModel>
    {
        const string EntryStateKey = "Entry";
        const string LabelStateKey = "Label";
        
        public EntryPage()
        {
            this.InitializeComponent();

            ViewModel = Initializer.GetSingleExport<EntryViewModel>();
        }

        public EntryViewModel ViewModel
        {
            get { return this.DataContext as EntryViewModel; }
            set { this.DataContext = value; }
        }

        public void End(Action completed)
        {
            completed();
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

        public string Location
        {
            get { return @"/Views/EntryPage.xaml"; }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        async void OnSecondaryTilePinButtonClick(object sender, RoutedEventArgs e)
        {
            string secondaryTileId = ViewModel.Entry.Id.ToString();
            string activationArguments = string.Format("Entry {0} pinned at {1}.", secondaryTileId, DateTime.Now.ToLocalTime().ToString());
            ISessionState sessionState = Initializer.GetSingleExport<ISessionState>();

            SecondaryTile secondaryTile = new SecondaryTile(secondaryTileId,
                                                "UOME",
                                                activationArguments,
                                                //TODO: Change this secondary tile logo
                                                new Uri("ms-appx:///Assets/square150x150Tile-sdk.png"),
                                                TileSize.Square150x150);
            
            secondaryTile.RoamingEnabled = false;
            secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;

            await ViewModel.PinEntry();

            await secondaryTile.RequestCreateAsync();

            //NOTE: Since the user is taken right to the start screen nothing here will run.
        }

        async void OnSecondaryTileUnpinButtonClick(object sender, RoutedEventArgs e)
        {
            SecondaryTile secondaryTile = new SecondaryTile(ViewModel.Entry.Id.ToString());
            bool wasUnpinned = await secondaryTile.RequestDeleteAsync();

            if (wasUnpinned)
            {
                await ViewModel.UnpinEntry();
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

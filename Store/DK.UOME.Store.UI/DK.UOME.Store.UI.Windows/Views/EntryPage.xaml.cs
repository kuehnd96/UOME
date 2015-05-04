using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Collections.Generic;
using System.Composition;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace DK.UOME.Store.UI.Windows.Views
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

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);

            Point point = buttonTransform.TransformPoint(new Point());

            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));

        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // DECISION: Need event handlers to get at sender button rendering rectangle for separation of platform-specific code
        async void OnSecondaryTilePinButtonClick(object sender, RoutedEventArgs e)
        {
            BottomAppBar.IsSticky = true;

            var senderRect = GetElementRect((FrameworkElement)sender);
            string secondaryTileId = ViewModel.Entry.Id.ToString();
            string activationArguments = string.Format("Entry {0} pinned at {1}.", secondaryTileId, DateTime.Now.ToLocalTime().ToString());

            SecondaryTile secondaryTile = new SecondaryTile(secondaryTileId,
                                                string.Format("{0}: {1}", ViewModel.Entry.ThingLabel, ViewModel.Entry.Thing),
                                                activationArguments,
                                                //TODO: Change this secondary tile logo
                                                new Uri("ms-appx:///Assets/square150x150Tile-sdk.png"),
                                                TileSize.Wide310x150);
            secondaryTile.RoamingEnabled = false;
            secondaryTile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;

            bool wasPinned = await secondaryTile.RequestCreateForSelectionAsync(senderRect, Placement.Above);

            if (wasPinned)
            {
                await ViewModel.PinEntry();
            }

            BottomAppBar.IsSticky = false;
        }

        async void OnSecondaryTileUnpinButtonClick(object sender, RoutedEventArgs e)
        {
            var senderRect = GetElementRect((FrameworkElement)sender);

            SecondaryTile secondaryTile = new SecondaryTile(ViewModel.Entry.Id.ToString());
            bool wasUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(senderRect, Placement.Above);

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

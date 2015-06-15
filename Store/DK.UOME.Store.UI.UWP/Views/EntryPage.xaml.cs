using DK.UOME.Store.UI.UWP.DesignData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DK.UOME.Store.UI.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EntryPage : Page
    {
        public EntryPage()
        {
            this.InitializeComponent();

#if DEBUG
            this.DataContext = new DesignEntryViewModel();
#endif
        }

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
    }
}

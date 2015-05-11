#if WINDOWS_PHONE_APP
using DK.UOME.Store.UI.WindowsPhone.Views;
using DK.Framework.Store.WinPhone8;
using DK.Framework.Store.WinPhone8.Controls;
#elif WINDOWS_APP
using DK.UOME.Store.UI.Windows.Views;
using DK.Framework.Store.Win8;
#elif WINDOWS_UAP
using Microsoft.ApplicationInsights;
#endif

using StoreFramework = DK.Framework.Store;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.Repositories;
using DK.UOME.Store.PresentationModel.MappingConfigurations;
using UIModel = DK.UOME.Store.UI.DataModel;
using StorageModel = DK.UOME.DataAccess.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using DK.UOME.DataAccess.Interfaces;
using System.Diagnostics;
using DK.Framework.Core.Interfaces;
using System.Composition;
using DK.Framework.Store;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.DataAccess.Local;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using DK.Framework.Store.Exceptions;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace DK.UOME.Store.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {

#if WINDOWS_PHONE_APP

        private TransitionCollection transitions;

        [Import]
        public ISessionState SessionState { get; set; }

#elif WINDOWS_UAP

        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static Microsoft.ApplicationInsights.TelemetryClient TelemetryClient;

#endif



        //[Import]
        //public StoreFramework.SuspensionManager SuspensionManager { get; set; }

        [Import]
        public INavigationService NavigationService { get; set; }

        [Import]
        public IEntryRepository EntryRepository { get; set; }

        /// <summary>
        /// Gets the root navigation frame in the application.
        /// </summary>
        public Frame RootFrame { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
#if WINDOWS_UAP
            TelemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();
#endif

#if !WINDOWS_UAP
            this.InitializeComponent(); 
#endif

            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            RootFrame = Window.Current.Content as Frame;

            StartComposition(RootFrame);

            AutoMapperConfiguration.Configure();

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            await UpdateSecondaryTiles();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();

                // Associate the frame with a SuspensionManager key
                SuspensionManager.RegisterFrame(RootFrame, "UOMEFrame");

                // Change this value to a cache size that is appropriate for your application
                RootFrame.CacheSize = 10;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = RootFrame;
            }

            if (RootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (RootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in RootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                RootFrame.ContentTransitions = null;
                RootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif
                // The navigation stack wasn't restored

                string launchTileId = e.TileId;
                int launchTileEntryId;

                // Check if this launch is from an entry secondary tile
                if (int.TryParse(launchTileId, out launchTileEntryId))
                {
                    // Go right to the entry screen
                    NavigationService.Navigate<IScreen<EntryViewModel>>(screen =>
                    {
                        screen.ViewModel.EntryId = launchTileEntryId;
                    }); 
                }
                else
                {
                    NavigationService.Navigate<IScreen<MainViewModel>>();
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var RootFrame = sender as Frame;
            RootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            RootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SuspensionManager.SaveAsync();

            deferral.Complete();
        }

        void StartComposition(object rootVisual)
        {
            CompositionStarter starter = new CompositionStarter(this, rootVisual);

            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(App).GetTypeInfo().Assembly,
                typeof(StorageModel.Entry).GetTypeInfo().Assembly, 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(EntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(DK.UOME.DataAccess.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(EntryDataAccess).GetTypeInfo().Assembly,
                typeof(MainViewModel).GetTypeInfo().Assembly};
            
            // Add platform-specific framework
#if WINDOWS_PHONE_APP
            compositionAssemblies.Add(typeof(ValidationMessageControl).GetTypeInfo().Assembly);
#endif            
            // Restore this once there is something in the Win8 store framework
            //#elif WINDOWS_APP
            //            compositionAssemblies.Add(typeof(Framework.Store.Win8.BasePage).GetTypeInfo().Assembly);
            //#endif    
            
            starter.Configure(compositionAssemblies, null);
        }

        async Task UpdateSecondaryTiles()
        {
            //DECISION: Could not run these in parallel because FindAllForPackageAsync wouldn't return
            var secondaryTiles = await SecondaryTile.FindAllForPackageAsync();
            var tileEntryIds = secondaryTiles.Select(pinnedSecondaryTile => int.Parse(pinnedSecondaryTile.TileId));
            var pinnedEntryIds = await EntryRepository.GetAllPinnedEntrtIds();

            // Process any tiles that were removed since last session
            // Iterate through all entry identifiers that are in the persisted storage as pinned but whose tile was unpinned
            foreach (var unpinnedEntryId in pinnedEntryIds.Where(pinnedEntryId => !tileEntryIds.Contains(pinnedEntryId)))
            {
                EntryRepository.UnpinEntry(unpinnedEntryId);
            }

            // Process any tiles that were added since last session
            // Iterate through all entry identifiers that have a secondary tile but aren't in the persisted storage
            foreach (var newlyPinnedTileId in tileEntryIds.Where(tileEntryId => !pinnedEntryIds.Contains(tileEntryId)))
            {
                EntryRepository.PinEntry(newlyPinnedTileId);
            }
        }
    }
}
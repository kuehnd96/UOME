using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.DataAccess.Local.UWP;
using DK.UOME.Store.PresentationModel.MappingConfigurations;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.Repositories;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StorageModel = DK.UOME.DataAccess.DataModel;

namespace DK.UOME.Store.UI.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
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
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
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

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (Framework.Store.Exceptions.SuspensionManagerException)
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
                NavigationService.Navigate<IScreen<MainViewModel>>();
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

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
                typeof(DK.UOME.Store.UI.DataModel.Entry).GetTypeInfo().Assembly,
                typeof(EntryDataAccess).GetTypeInfo().Assembly,
                typeof(MainViewModel).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);
        }
    }
}

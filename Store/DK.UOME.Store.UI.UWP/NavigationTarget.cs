using DK.Framework.Core;
using DK.Framework.Core.Interfaces;
using System;
using System.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DK.UOME.Store.UI.UWP
{
    /// <summary>
    /// Navigation target for the <see cref="INavigationService">navigation service</see>.
    /// </summary>
    /// <remarks>Acts as a proxy to avoid creating a second instance of App for the navigation service.</remarks>
    [Export(typeof(INavigationTarget))]
    [Shared]
    public class NavigationTarget : INavigationTarget
    {
        Frame _rootFrame;

        /// <summary>
        /// Gets the <see cref="IScreen"/> instance that is being displayed.
        /// </summary>
        public IScreen Current
        {
            get { return _rootFrame.Content as IScreen; }
        }

        /// <summary>
        /// Should only be used by MEF.
        /// </summary>
        public NavigationTarget()
        {
            _rootFrame = ((App)App.Current).RootFrame;
        }

        /// <summary>
        /// Reverts to the previous screen.
        /// </summary>
        public void GoBack()
        {
            if (_rootFrame != null && CanGoBack())
            {
                _rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Goes forward to the next screen (if any).
        /// </summary>
        public void GoForward()
        {
            if (_rootFrame != null && CanGoForward())
            {
                _rootFrame.GoForward();
            }
        }

        /// <summary>
        /// Indicates whether or not backward navigation is possible.
        /// </summary>
        /// <returns>True if it is possible; Otherwise false.</returns>
        public bool CanGoBack()
        {
            return _rootFrame.CanGoBack;
        }

        /// <summary>
        /// Indicates whether or not forward navigation is possible.
        /// </summary>
        /// <returns>True if it is possible; Otherwise false.</returns>
        public bool CanGoForward()
        {
            return _rootFrame.CanGoForward;
        }

        /// <summary>
        /// Goes all the way to the first screen in the application session.
        /// </summary>
        public void GoHome()
        {
            if (_rootFrame != null)
            {
                while (_rootFrame.CanGoBack) _rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Used by the navigation service. Navigates to a screen and performs and initialization action.
        /// </summary>
        /// <param name="screen">The <see cref="IScreen"/> instance.</param>
        /// <param name="initAction">The actiont to perform upon naviagtion.</param>
        public void Show(IScreen screen, Action<IScreen> initAction)
        {
            Requires.IsNotNull(screen, "Screen parameter is null.");
            Requires.IsNotNull(initAction, "Init action parameter is null.");

            // We quickly assign a handler to run the initialization action of navigating
            NavigatedEventHandler navigatedAction = null;
            navigatedAction = (sender, args) =>
            {
                IScreen nextScreen = args.Content as IScreen;

                if (null != nextScreen)
                {
                    initAction(nextScreen);
                }

                // .. and then remove the event handler at the end of execution
                _rootFrame.Navigated -= navigatedAction;
            };

            _rootFrame.Navigated += navigatedAction;
            _rootFrame.Navigate(screen.ScreenType);
        }

        /// <summary>
        /// Used by the navigation service. Navigates to a screen.
        /// </summary>
        /// <param name="screen"></param>
        public void Show(IScreen screen)
        {
            Requires.IsNotNull(screen, "Screen parameter is null.");

            _rootFrame.Navigate(screen.ScreenType);
        }
    }
}

using DK.Framework.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.UOME.Store.Windows.Tests.Fakes
{
    /// <summary>
    /// This is just a placeholder to appease composition for the unit tests. Do not use!
    /// </summary>
    [Export(typeof(INavigationTarget))]
    [Shared]
    public class DummyNavigationTarget : INavigationTarget
    {
        readonly Stack<IScreen> _pastScreens = new Stack<IScreen>();
        readonly Stack<IScreen> _futureScreens = new Stack<IScreen>();
        
        /// <summary>
        /// Gets the <see cref="IScreen"/> instance that is being displayed.
        /// </summary>
        public IScreen Current { get; private set; }

        /// <summary>
        /// Should only be used by MEF.
        /// </summary>
        public DummyNavigationTarget()
        {
        }

        /// <summary>
        /// Reverts to the previous screen.
        /// </summary>
        public void GoBack()
        {
            var lastScreen = _pastScreens.Pop();

            Current = lastScreen;
        }

        /// <summary>
        /// Goes forward to the next screen (if any).
        /// </summary>
        public void GoForward()
        {
            var nextScreen = _futureScreens.Pop();

            Current = nextScreen;
        }

        /// <summary>
        /// Indicates whether or not backward navigation is possible.
        /// </summary>
        /// <returns>True if it is possible; Otherwise false.</returns>
        public bool CanGoBack()
        {
            return _pastScreens.Any();
        }

        /// <summary>
        /// Indicates whether or not forward navigation is possible.
        /// </summary>
        /// <returns>True if it is possible; Otherwise false.</returns>
        public bool CanGoForward()
        {
            return _futureScreens.Any();
        }

        /// <summary>
        /// Goes all the way to the first screen in the application session.
        /// </summary>
        public void GoHome()
        {
            IScreen firstScreen = null;

            while (_pastScreens.Any())
            {
                firstScreen = _pastScreens.Pop();
            }

            Current = firstScreen;
        }

        /// <summary>
        /// Used by the navigation service. Navigates to a screen and performs and initialization action.
        /// </summary>
        /// <param name="screen">The <see cref="IScreen"/> instance.</param>
        /// <param name="initAction">The actiont to perform upon naviagtion.</param>
        public void Show(IScreen screen, Action<IScreen> initAction)
        {
            if (Current != null)
            {
                _pastScreens.Push(Current);
            }

            _futureScreens.Clear();
            
            Current = screen;

            if (initAction != null)
            {
                initAction(screen);
            }
        }

        /// <summary>
        /// Used by the navigation service. Navigates to a screen.
        /// </summary>
        /// <param name="screen"></param>
        public void Show(IScreen screen)
        {
            if (Current != null)
            {
                _pastScreens.Push(Current);
            }

            _futureScreens.Clear();

            Current = screen;
        }
    }
}

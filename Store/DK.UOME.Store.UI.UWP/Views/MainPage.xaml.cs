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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

#if DEBUG
            this.DataContext = new DesignMainViewModel();
#endif
        }

        private void OnMenuButtonClick(object sender, RoutedEventArgs e)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

        private void OnNavListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SplitView.IsPaneOpen = false;
        }

        private void OnNavListLoaded(object sender, RoutedEventArgs e)
        {
            //NOTE: Why does this run twice?

            var listView = sender as ListView;

            if ((listView != null) &&
                (listView.Items.Any()))
            {
                listView.SelectedIndex = 0;
            }
        }
    }
}

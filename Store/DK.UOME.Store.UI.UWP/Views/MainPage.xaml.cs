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
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DK.UOME.Store.UI.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Screen(typeof(IScreen<MainViewModel>))]
    [Shared]
    public sealed partial class MainPage : BaseStorePage, IScreen<MainViewModel>
    {
        public MainViewModel ViewModel
        {
            get
            {
                return DataContext as MainViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        public Type ScreenType
        {
            get { return typeof(MainPage); }
        }

        public string Location { get { return "/MainPage.xaml"; } }

        public MainPage()
        {
            this.InitializeComponent();

#if DEBUG
            this.DataContext = new DesignMainViewModel();
#else
            //ViewModel = Initializer.GetSingleExport<MainViewModel>();
#endif
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void Start(Action completed)
        {
            //LIVETILE
            // We must be on the lock screen
            //var status = await BackgroundExecutionManager.RequestAccessAsync();

            //switch (status)
            //{
            //    case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
            //    case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
            //        RegisterBackgroundTasks();
            //        break;

            //    case BackgroundAccessStatus.Denied:
            //    case BackgroundAccessStatus.Unspecified:
            //    default:
            //        break;
            //}

            //TODO: Uncomment this to load real data
            //await ViewModel.Start();

            completed();
        }

        public void End(Action completed)
        {
            completed();
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            // No state to load on this page
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            // No state to save on this page
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEntry = e.AddedItems.FirstOrDefault();

            if (selectedEntry != null)
            {
                ViewModel.NavigateEntry(selectedEntry as Entry);
            }
        }
    }
}

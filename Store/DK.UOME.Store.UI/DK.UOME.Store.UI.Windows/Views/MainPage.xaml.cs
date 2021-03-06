﻿using DK.Framework.Core.Interfaces;
using DK.Framework.Store;
using DK.Framework.Store.Attributes;
using DK.Framework.Store.Model;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.UI.DataModel;
using System;
using System.Composition;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Controls;

namespace DK.UOME.Store.UI.Windows.Views
{
    [Screen(typeof(IScreen<MainViewModel>))]
    [Shared]
    public sealed partial class MainPage : BaseStorePage, IScreen<MainViewModel>
    {
        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = Initializer.GetSingleExport<MainViewModel>();
        }

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

        public void End(Action completed)
        {
            completed();
        }

        public Type ScreenType
        {
            get { return typeof(MainPage); }
        }

        public string Location { get { return "/EntryPage.xaml"; } }

        public async void Start(Action completed)
        {
            // We must be on the lock screen
            var status = await BackgroundExecutionManager.RequestAccessAsync();

            switch (status)
            {
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                    RegisterBackgroundTasks();
                    break;

                case BackgroundAccessStatus.Denied:
                case BackgroundAccessStatus.Unspecified:
                default:
                    break;
            }
            
            await ViewModel.Start();

            completed();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnEntryItemClicked(object sender, ItemClickEventArgs e)
        {
            var landingItem = e.ClickedItem as LandingItem<Entry>;

            if (null != landingItem)
            {
                if (landingItem.HasItem)
                {
                    ViewModel.NavigateEntry(landingItem.Thing);
                }
                else
                {
                    var clickedGroup = landingItem.Tag as EntryGroup;

                    if (null != clickedGroup)
                    {
                        ViewModel.NavigateGroup(clickedGroup);
                    }
                }
            }
        }

        void RegisterBackgroundTasks()
        {
            const string entryTaskName = "EntryBackgroundTask";
            const string entryTaskEntryPoint = "DK.UOME.Store.Background.EntryBackgroundTask";
#if DEBUG
            const uint entryTaskInterval = 15; // in minutes
#else
            const uint entryTaskInterval = 30; // in minutes
#endif

            bool isEntryTaskRegistered = false;

            foreach (var registeredTask in BackgroundTaskRegistration.AllTasks)
            {
                if (registeredTask.Value.Name == entryTaskName)
                {
                    isEntryTaskRegistered = true;
                    break;
                }
            }

            if (!isEntryTaskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = entryTaskName;
                builder.TaskEntryPoint = entryTaskEntryPoint;
                builder.SetTrigger(new TimeTrigger(entryTaskInterval, false));

                BackgroundTaskRegistration taskRegistration = builder.Register();
            }
        }

        private void OnSearchQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            ViewModel.PerformSearch(args.QueryText.Trim());
        }
    }
}

using DK.Framework.Core;
using DK.Framework.Core.Interfaces;
using DK.Framework.Store.Commands;
using DK.Framework.Store.Interfaces;
using DK.Framework.Store.Model;
using DK.UOME.Repositories.Interfaces;
using UIModel = DK.UOME.Store.UI.DataModel;
using StorageModel = DK.UOME.DataAccess.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.PresentationModel.MappingConfigurations.Profiles;
using Windows.UI.Notifications;
using DK.UOME.Store.Notifications;

namespace DK.UOME.Store.PresentationModel.ViewModels
{
    /// <summary>
    /// View logic for the main landing page of entries.
    /// </summary>
    [Export(typeof(MainViewModel))]
    [Shared]
    public class MainViewModel : ShowEntryViewModel
    {
        const int EntryMax = 5;
        const string NewestBorrowedGroupName = "Newest Borrowed";
        const string NewestLoanedGroupName = "Newest Loaned";
        const string OverdueGroupName = "Overdue";
        const string NextDueGroupName = "Next Due";
        const string NavigationItemLabel = "View All";

        ObservableCollection<UIModel.EntryGroup> _entryGroups;
        DelegateCommand _navigateGroupCommand;
        UIModel.EntryGroup _overdueEntryGroup = new UIModel.EntryGroup();
        UIModel.EntryGroup _nextDueEntryGroup = new UIModel.EntryGroup();
        UIModel.EntryGroup _borrowedEntryGroup = new UIModel.EntryGroup();
        UIModel.EntryGroup _loanedEntryGroup = new UIModel.EntryGroup();
        bool _hasLoaded = false;

        //CLOUD
        //[Import]
        //public IMobileServiceClient AuthenticationClient { get; set; }

        /// <summary>
        /// Gets or sets the groups of entries to display.
        /// </summary>
        public ObservableCollection<UIModel.EntryGroup> EntryGroups
        {
            get { return _entryGroups; }
            set { SetProperty(ref _entryGroups, value); }
        }

        /// <summary>
        /// Gets a command that navigates to an entry group.
        /// </summary>
        public DelegateCommand NavigateGroupCommand
        {
            get
            {
                if (null == _navigateGroupCommand)
                {
                    _navigateGroupCommand = new DelegateCommand(NavigateGroup);
                }
                
                return _navigateGroupCommand;
            }
        }

        public bool UseEntryMax { get; set; }

        /// <summary>
        /// Functionality that should run every time the screen is navigated to.
        /// </summary>
        /// <returns>Async task</returns>
        public async Task Start()
        {
            //CLOUD: Should this only run in cloud mode?
            //if (!AuthenticationClient.IsAuthenticated)
            //{
            //    await AuthenticationClient.Authenticate(MobileServiceAuthenticationProvider.MicrosoftAccount);
            //}

            if (!_hasLoaded)
            {
                await LoadEntries();

                _hasLoaded = true;
            }
        }

        /// <summary>
        /// Loads entries and sets up update actions.
        /// </summary>
        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            EntryRepository.UpdateActions.Add(LoadEntries);
        }

        async Task LoadEntries()
        {
            var rawEntries = await EntryRepository.GetAllEntries();
            var entries = EntryModelProfile.MapEntryList(rawEntries);
            var today = DateTime.Today;
            IList<UIModel.EntryGroup> entryGroups = new List<UIModel.EntryGroup>(4);

            // Get entries that are overdue
            var overdueEntries = from entry in entries
                                 where entry.DueDate.HasValue && entry.DueDate < today
                                 orderby entry.DueDate
                                 select entry;

            entryGroups.Add(CreateEntryGroup(overdueEntries, OverdueGroupName, null));

            // Get the entries that have due dates
            var nextDueEntries = from entry in entries
                                  where entry.DueDate.HasValue && entry.DueDate >= today
                                  orderby entry.DueDate
                                  select entry;

            entryGroups.Add(CreateEntryGroup(nextDueEntries, NextDueGroupName, null));
            
            // Get all borrowed items
            var borrowedEntries = from entry in entries
                                  where entry.Type == UIModel.EntryType.Borrowed
                                  orderby entry.DueDate.HasValue ? entry.DueDate : DateTime.MaxValue
                                  select entry;

            entryGroups.Add(CreateEntryGroup(borrowedEntries, NewestBorrowedGroupName, UIModel.EntryType.Borrowed));
            
            // Get all loaned entries
            var loanedEntries = from entry in entries
                                where entry.Type == UIModel.EntryType.Loaned
                                orderby entry.DueDate.HasValue ? entry.DueDate : DateTime.MaxValue
                                select entry;

            entryGroups.Add(CreateEntryGroup(loanedEntries, NewestLoanedGroupName, UIModel.EntryType.Loaned));

            EntryGroups = new ObservableCollection<UIModel.EntryGroup>(entryGroups);

            var pinnedEntyIds = await EntryRepository.GetAllPinnedEntrtIds();

            SendNotifications(rawEntries, pinnedEntyIds);
        }

        protected EntryGroup CreateEntryGroup(IEnumerable<UIModel.Entry> entries, string groupName, UIModel.EntryType? groupType = null, string icon = null)
        {
            var entryGroup = new UIModel.EntryGroup() { Name = groupName };
            entryGroup.LandingItems = new ObservableCollection<LandingItem<UIModel.Entry>>(ConvertEntriesToLandingItems(entries.Take(EntryMax)));
            entryGroup.Items = new ObservableCollection<UIModel.Entry>(entries);
            entryGroup.Type = groupType;
            entryGroup.Icon = icon;

            if ((UseEntryMax) && (entries.Count() > EntryMax))
            {
                // We need to add a placeholder indicating there are more items in this group
                entryGroup.LandingItems.Add(new LandingItem<UIModel.Entry>(NavigationItemLabel) { Tag = entryGroup });
            }

            return entryGroup;
        }

        IEnumerable<LandingItem<UIModel.Entry>> ConvertEntriesToLandingItems(IEnumerable<UIModel.Entry> entries)
        {
            var convertedEntries = new List<LandingItem<UIModel.Entry>>(entries.Count());

            foreach (var entry in entries)
            {
                convertedEntries.Add(new LandingItem<UIModel.Entry>(entry));
            }

            return convertedEntries;
        }

        /// <summary>
        /// Navigates to a group of entries.
        /// </summary>
        /// <param name="parameter"></param>
        public void NavigateGroup(object parameter)
        {
            Requires.IsNotNull(parameter, "Group parameter is null.");

            var group = parameter as UIModel.EntryGroup;
            
            Navigation.Navigate<IScreen<EntryGroupViewModel>>(screen => screen.ViewModel.EntryGroup = group);
        }

        protected override bool CanDeleteEntries(object parameter)
        {
            // Deletion not allowed in this screen
            return false;
        }

        void SendNotifications(IList<StorageModel.Entry> entries, IList<int> pinnedEntryIds)
        {
            var tileEngine = new EntryNotificationEngine(entries, pinnedEntryIds);
            var mainTiles = tileEngine.GetMainTiles();
            var secondaryTiles = tileEngine.GetSecondaryTiles();
            var badges = tileEngine.GetBadges();

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            foreach (var mainTile in mainTiles)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Update(mainTile.CreateNotification());
            }

            foreach (var secondaryTile in secondaryTiles)
            {
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryTile.Key.ToString()).Clear();

                TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryTile.Key.ToString())
                    .Update(secondaryTile.Value.CreateNotification());
            }

            foreach (var updatedBadge in badges)
            {
                if (updatedBadge.Key == EntryNotificationEngine.MainBadgeId)
                {
                    BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

                    BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(updatedBadge.Value.CreateNotification());
                }
                else
                {
                    string badgeId = updatedBadge.Key.ToString();

                    BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(badgeId).Clear();

                    BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(badgeId).Update(updatedBadge.Value.CreateNotification());
                }
            }
        }
    }
}

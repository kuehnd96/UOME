using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.DataAccess.Local;
using DK.UOME.Store.Notifications;
using DK.UOME.Store.Repositories;
using System.Collections.Generic;
using System.Reflection;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace DK.UOME.Store.Background
{
    /// <summary>
    /// Task for updating tiles and badges when UOME isn't running.
    /// </summary>
    public sealed class EntryBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            StartComposition();
            
            var tileEngine = new EntryNotificationEngine();
            var mainTiles = tileEngine.GetMainTiles();
            var secondaryTiles = tileEngine.GetSecondaryTiles();
            var badges = tileEngine.GetBadges();

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            foreach (var mainTile in mainTiles)
            {
                TileUpdateManager.CreateTileUpdaterForApplication()
                    .Update(mainTile.CreateNotification());
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

        void StartComposition()
        {
            CompositionStarter starter = new CompositionStarter(this, null);

            // We need to get MEF going!
            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(EntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(EntryDataAccess).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);
        }
    }
}

using DK.Framework.Store;
using DK.Framework.Store.NotificationsExtensions.BadgeContent;
using DK.Framework.Store.NotificationsExtensions.TileContent;
using DK.UOME.DataAccess.DataModel;
using DK.UOME.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DK.UOME.Store.Notifications
{
    /// <summary>
    /// Produces notifications for tiles and badges.
    /// </summary>
    /// <remarks>This class is designed for a short life-cycle.</remarks>
    public sealed class EntryNotificationEngine
    {
        const string AppLabel = "UOME";
        const string OverdueLabel = "Overdue";
        const string DueTodayLabel = "Due Today";
        const string LongDateFormat = "D";
        const string ShortDateFormat = "d";
        public const int MainBadgeId = -1;
        
        readonly IList<Entry> _entries = null;
        readonly IList<int> _pinnedEntryIds = null;
        readonly DateTime _today = DateTime.Today;

        /// <summary>
        /// Creates an instance and loads entries from repository.
        /// </summary>
        public EntryNotificationEngine()
        {
            IEntryRepository repository = Initializer.GetSingleExport<IEntryRepository>();

            var getAllEntriesTask = repository.GetAllEntries();
            var getPinnedEntryIdsTask = repository.GetAllPinnedEntrtIds();

            Task.WaitAll(new Task[] { getAllEntriesTask, getPinnedEntryIdsTask });
            
            _entries = getAllEntriesTask.Result;
            _pinnedEntryIds = new List<int>(getPinnedEntryIdsTask.Result);
        }

        /// <summary>
        /// Creates an instance with the provided entries.
        /// </summary>
        /// <param name="entries">List of all entries. Cannot be null.</param>
        public EntryNotificationEngine(IList<Entry> entries, IList<int> pinnedEntryIds)
        {
            if (entries == null)
            {
                throw new ArgumentNullException("entries");
            }

            if (pinnedEntryIds == null)
            {
                throw new ArgumentNullException("pinnedEntryIds");
            }

            _entries = entries;
            _pinnedEntryIds = pinnedEntryIds;
        }

        /// <summary>
        /// Produces live tiles for the main application tile.
        /// </summary>
        /// <returns>Collection of updated tiles for the main tile.</returns>
        public IList<ITileNotificationContent> GetMainTiles()
        {
            var tiles = new List<ITileNotificationContent>(2);
            var overdueEntries = _entries.Where(entry => entry.DueDate.HasValue && entry.DueDate.Value < _today);
            var entriesDueToday = _entries.Where(entry => entry.DueDate.HasValue && entry.DueDate.Value == _today);

            if (overdueEntries.Any())
            {
                var overdueTile = CreateMainEntryTile(overdueEntries.ToList(), OverdueLabel);
                tiles.Add(overdueTile);
            }
            
            if (entriesDueToday.Any())
            {
                var dueTodayTile = CreateMainEntryTile(entriesDueToday.ToList(), DueTodayLabel);
                tiles.Add(dueTodayTile);
            }

            return tiles;
        }

        /// <summary>
        /// Produces updated tiles for all pinned entries.
        /// </summary>
        /// <returns>A collection of updated tile content.</returns>
        public IDictionary<int, ITileNotificationContent> GetSecondaryTiles()
        {
            var tiles = new Dictionary<int, ITileNotificationContent>();

            foreach (int pinnedEntryId in _pinnedEntryIds)
            {
                var pinnedEntry = _entries.Where(entry => entry.Id == pinnedEntryId).FirstOrDefault();

                if (pinnedEntry != null)
                {
                    var pinnedEntryTile = CreateSecondaryEntryTile(pinnedEntry);
                    tiles.Add(pinnedEntryId, pinnedEntryTile);
                }
            }

            return tiles;
        }

        public IDictionary<int, BadgeGlyphNotificationContent> GetBadges()
        {
            var badges = new Dictionary<int, BadgeGlyphNotificationContent>();
            var overdueEntryIds = _entries.Where(entry => entry.DueDate.HasValue && entry.DueDate.Value < _today).Select(entry => entry.Id);
            var entriesDueToday = _entries.Where(entry => entry.DueDate.HasValue && entry.DueDate.Value == _today).Select(entry => entry.Id);

            // Create main badge
            if (entriesDueToday.Any())
            {
                badges.Add(MainBadgeId, new BadgeGlyphNotificationContent(GlyphValue.Alert));
            }
            else if (overdueEntryIds.Any())
            {
                badges.Add(MainBadgeId, new BadgeGlyphNotificationContent(GlyphValue.Attention));
            }

            // Create secondary tile badges
            foreach (int pinnedEntryId in _pinnedEntryIds)
            {
                if (entriesDueToday.Contains(pinnedEntryId))
                {
                    badges.Add(pinnedEntryId, new BadgeGlyphNotificationContent(GlyphValue.Alert));
                }

                if (overdueEntryIds.Contains(pinnedEntryId))
                {
                    badges.Add(pinnedEntryId, new BadgeGlyphNotificationContent(GlyphValue.Attention));
                }
            }

            return badges;
        }

        ITileNotificationContent CreateMainEntryTile(IList<Entry> entries, string label)
        {
            if (entries.Count > 1)
            {
                string entryCount = entries.Count.ToString();
                
                //var tinyTile = TileContentFactory.CreateTileSquare71x71IconWithBadge();
                //TODO: Get Windows Phone main tile working w/ badge

                var squareTile = TileContentFactory.CreateTileSquare150x150Block();
                squareTile.RequireSquare71x71Content = false;
                squareTile.TextBlock.Text = entryCount;
                squareTile.TextSubBlock.Text = label;

                var wideTile = TileContentFactory.CreateTileWide310x150BlockAndText02();
                wideTile.Square150x150Content = squareTile;
                wideTile.TextBodyWrap.Text = string.Format("Multiple entries are {0}", label);
                wideTile.TextBlock.Text = entryCount;
                wideTile.TextSubBlock.Text = AppLabel;

                var largeTile = TileContentFactory.CreateTileSquare310x310BlockAndText01();
                largeTile.Wide310x150Content = wideTile;
                largeTile.TextHeadingWrap.Text = string.Format("Multiple entries are {0}", label);
                largeTile.TextSubBlock.Text = AppLabel;
                largeTile.TextBlock.Text = entryCount;
                
                var nextEntries = entries.OrderBy(entry => entry.DueDate).Take(2);
                largeTile.TextBody3.Text = nextEntries.First().Thing;
                largeTile.TextBody4.Text = nextEntries.First().DueDate.Value.ToString(LongDateFormat);

                var secondEntry = nextEntries.Skip(1).Take(1).Single();
                largeTile.TextBody5.Text = secondEntry.Thing;
                largeTile.TextBody6.Text = secondEntry.DueDate.Value.ToString(LongDateFormat);

                return largeTile;
            }
            else // Single entry
            {
                var entry = entries.First();

                var squareTile = TileContentFactory.CreateTileSquare150x150Text01();
                squareTile.RequireSquare71x71Content = false;
                squareTile.TextBody1.Text = label;
                squareTile.TextBody2.Text = entry.Thing;
                squareTile.TextBody3.Text = entry.DueDate.Value.ToString(ShortDateFormat);

                var wideTile = TileContentFactory.CreateTileWide310x150Text01();
                wideTile.Square150x150Content = squareTile;
                wideTile.TextHeading.Text = label;
                wideTile.TextBody1.Text = entry.Thing;
                wideTile.TextBody2.Text = entry.DueDate.Value.ToString(LongDateFormat);
                wideTile.TextBody3.Text = entry.OtherParty;

                var largeTile = TileContentFactory.CreateTileSquare310x310Text09();
                largeTile.Wide310x150Content = wideTile;
                largeTile.TextHeadingWrap.Text = entry.Thing;
                largeTile.TextHeading1.Text = label;
                largeTile.TextBody1.Text = entry.DueDate.Value.ToString(LongDateFormat);
                largeTile.TextBody2.Text = entry.OtherParty;

                return largeTile;
            }
        }

        ITileNotificationContent CreateSecondaryEntryTile(Entry entry)
        {
            bool hasDueDate = entry.DueDate.HasValue;

            var squareTile = TileContentFactory.CreateTileSquare150x150Text01();
            squareTile.RequireSquare71x71Content = false;
            squareTile.TextHeading.Text = entry.TypeLabel;
            squareTile.TextBody1.Text = entry.Thing;
            squareTile.TextBody2.Text = entry.OtherParty;

            if (hasDueDate)
            {
                squareTile.TextBody3.Text = entry.DueDate.Value.ToString(); 
            }

            var wideTile = TileContentFactory.CreateTileWide310x150Text01();
            wideTile.Square150x150Content = squareTile;
            wideTile.TextHeading.Text = entry.TypeLabel;
            wideTile.TextBody1.Text = entry.Thing;
            wideTile.TextBody2.Text = entry.OtherParty;

            if (hasDueDate)
            {
                wideTile.TextBody3.Text = entry.DueDate.Value.ToString(LongDateFormat);
            }

            var largeTile = TileContentFactory.CreateTileSquare310x310Text09();
            largeTile.Wide310x150Content = wideTile;
            largeTile.TextHeadingWrap.Text = entry.Thing;
            largeTile.TextHeading1.Text = entry.TypeLabel;
            largeTile.TextHeading2.Text = entry.OtherParty;
            
            if (hasDueDate)
            {
                largeTile.TextBody1.Text = entry.DueDate.Value.ToString(LongDateFormat);
            }

            if (!string.IsNullOrEmpty(entry.Note))
            {
                largeTile.TextBody2.Text = entry.Note;
            }

            return largeTile;
        }
    }
}

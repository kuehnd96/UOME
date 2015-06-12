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
using DK.UOME.Store.PresentationModel.MappingConfigurations.Profiles;
using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.PresentationModel.ViewModels;

namespace DK.UOME.Store.UI.UWP.DesignData
{
    public class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel()
        {
            IList<UIModel.EntryGroup> entryGroups = new List<UIModel.EntryGroup>(3);
            DateTime today = DateTime.Now.Date;

            Entry movieEntry = new BorrowedEntry()
            {
                CreateDate = today.AddDays(-22),
                DueDate = today.AddDays(3),
                Id = 1,
                OtherParty = "Jon Kuehn",
                Thing = "Despicable Me 2 Blu Ray",
                Note = "Return insert, too."
            };

            Entry videoGameEntry = new BorrowedEntry()
            {
                CreateDate = today.AddDays(-4),
                Id = 2,
                OtherParty = "Bob Zurad",
                Thing = "Titanfall",
                Note = "Xbox One disc game",
                DueDate = today.AddDays(1)
            };

            Entry foodEntry = new LoanedEntry()
            {
                CreateDate = today.AddDays(-40),
                Id = 3,
                OtherParty = "Kate O'Donnell",
                Thing = "$6",
                Note = "Appetizers at Ward's"
            };

            Entry phoneChargerEntry = new LoanedEntry()
            {
                CreateDate = today.AddDays(-10),
                Id = 4,
                OtherParty = "Andy Leichle",
                Thing = "Lumia Phone Charger"
            };

            Entry lunchMoneyEntry = new BorrowedEntry()
            {
                CreateDate = today,
                Id = 5,
                OtherParty = "Rob Sheehy",
                Thing = "$8",
                Note = "Spotted me money for lunch",
                DueDate = today.AddDays(14)
            };

            Entry wiiGameEntry = new BorrowedEntry()
            {
                CreateDate = today.AddDays(-50),
                Id = 6,
                OtherParty = "Eric Elser",
                Thing = "Super Mario Brothers Wii",
                Note = "Wii disc game"
            };

            entryGroups.Add(CreateEntryGroup(new List<Entry>() { phoneChargerEntry}, "Overdue", null, "NavigateOverdue"));
            entryGroups.Add(CreateEntryGroup(new List<Entry>() { movieEntry }, "Next Due", null, "NavigateNextDue"));
            entryGroups.Add(CreateEntryGroup(new List<Entry>() { movieEntry }, "Borrowed Group", EntryType.Borrowed, "NavigateBorrowed"));
            entryGroups.Add(CreateEntryGroup(new List<Entry>() { foodEntry }, "Loaned Group", EntryType.Loaned, "NavigateLoaned"));
            entryGroups.Add(CreateEntryGroup(new List<Entry>() { movieEntry, videoGameEntry, foodEntry, phoneChargerEntry, lunchMoneyEntry, wiiGameEntry }, "Mixed Group", null, "NavigateOverdue"));

            EntryGroups = new ObservableCollection<UIModel.EntryGroup>(entryGroups);
        }
    }
}

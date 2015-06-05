using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.PresentationModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK.UOME.Store.UI.UWP.Mobile.DesignData
{
    public class DesignEntryGroupViewModel : EntryGroupViewModel
    {
        public DesignEntryGroupViewModel()
        {
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

            EntryGroup group = new EntryGroup { Name = "Test Mixed Group" };
            group.Items = new ObservableCollection<Entry> { movieEntry, videoGameEntry, foodEntry, phoneChargerEntry, lunchMoneyEntry, wiiGameEntry };

            EntryGroup = group;
        }
    }
}

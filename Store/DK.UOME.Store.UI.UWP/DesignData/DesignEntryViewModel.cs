using DK.UOME.Store.PresentationModel.UWP.ViewModels;
using DK.UOME.Store.UI.DataModel.UWP;
using System;

namespace DK.UOME.Store.UI.UWP.DesignData
{
    public class DesignEntryViewModel : EntryViewModel
    {
        public DesignEntryViewModel()
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

            Entry = movieEntry;

            Label = "Borrowed Entry";
        }
    }
}

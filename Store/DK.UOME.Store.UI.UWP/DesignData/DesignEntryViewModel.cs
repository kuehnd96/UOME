using DK.UOME.Store.UI.DataModel;
using DK.UOME.Store.PresentationModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

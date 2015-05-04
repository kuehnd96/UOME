using DK.UOME.Web.UI.DataModel;
using UIModel = DK.UOME.Web.UI.DataModel;

namespace DK.UOME.Web.UI.Controllers
{
    public class BorrowedEntryController : EntryController<UIModel.BorrowedEntry>
    {
        public override EntryType EntryType
        {
            get { return UIModel.EntryType.Borrowed; }
        }
	}
}
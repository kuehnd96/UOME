using System.Collections.Generic;
using UIModel = DK.UOME.Web.UI.DataModel;

namespace DK.UOME.Web.UI.Models
{
    /// <summary>
    /// Model for the home index view.
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Gets or sets the entries that are overdue.
        /// </summary>
        public List<UIModel.Entry> OverdueEntries { get; set; }
        
        /// <summary>
        /// Gets or sets the entries that are next due.
        /// </summary>
        public List<UIModel.Entry> NextDueEntries { get; set; }
    }
}
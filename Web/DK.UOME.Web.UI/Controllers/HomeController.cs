using DK.UOME.Repositories.Interfaces;
using DK.UOME.Web.UI.MappingConfigurations.Profiles;
using DK.UOME.Web.UI.Models;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DK.UOME.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        [Import]
        public IEntryRepository EntryRepository { get; set; }

        public async Task<ActionResult> Index()
        {
            var items = await EntryRepository.GetAllEntries();
            var entries = EntryModelProfile.MapEntryList(items);
            var viewModel = new HomeViewModel();
            var today = DateTime.Today;

            // Get the next 5 entries due
            viewModel.NextDueEntries = (from entry in entries
                                       where entry.DueDate.HasValue 
                                         && entry.DueDate >= today
                                       orderby entry.DueDate ascending
                                       select entry).Take(5).ToList();

            // Get all overdue entries 
            viewModel.OverdueEntries = (from entry in entries
                                        where entry.DueDate.HasValue && entry.DueDate < today
                                        orderby entry.DueDate ascending
                                        select entry).ToList();
            
            return View(viewModel);
        }

        public ActionResult About()
        {
            // TODO: Add an about screen
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}
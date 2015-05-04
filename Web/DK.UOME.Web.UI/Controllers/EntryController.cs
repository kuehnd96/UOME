using AutoMapper;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Web.UI.MappingConfigurations.Profiles;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Web.UI.DataModel;

namespace DK.UOME.Web.UI.Controllers
{
    public abstract class EntryController<UIEntryType> : Controller where UIEntryType : UIModel.Entry, new()
    {
        [Import]
        public IEntryRepository EntryRepository { get; set; }

        public abstract UIModel.EntryType EntryType { get; }

        public virtual async Task<ActionResult> Index()
        {
            var entries = await EntryRepository.GetAllEntries();
            var entriesOfType = EntryModelProfile.MapEntryList(entries.Where(entry => entry.Type == (int)EntryType).ToList());

            return View(entriesOfType);
        }

        public async Task<ActionResult> Edit(int Id)
        {
            var entries = await EntryRepository.GetAllEntries();
            var dataEntry = entries.Where(item => item.Id == Id).FirstOrDefault();

            if (dataEntry == null)
            {
                return HttpNotFound();
            }

            UIModel.Entry entry = Mapper.Map<UIEntryType>(dataEntry);

            return View(entry);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UIEntryType updatedEntry)
        {
            if (ModelState.IsValid)
            {
                var entry = Mapper.Map<StorageModel.Entry>(updatedEntry);
                await EntryRepository.SaveEntry(entry);

                // Return to edit view
                return RedirectToAction("Get", updatedEntry.Id);
            }

            return View(updatedEntry);
        }

        public async Task<ActionResult> Delete(int Id)
        {
            var entries = await EntryRepository.GetAllEntries();
            var dataEntry = entries.Where(item => item.Id == Id);

            if (dataEntry == null)
            {
                return HttpNotFound();
            }

            UIModel.Entry entry = Mapper.Map<UIEntryType>(dataEntry);

            return View(entry);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int Id)
        {
            bool exists = await EntryRepository.EntryExists(Id);

            if (!exists)
            {
                return HttpNotFound();
            }
            
            await EntryRepository.DeleteEntry(Id);
            
            return RedirectToAction("Index", "Home");
        }

        public virtual ActionResult Create()
        {
            UIEntryType blankEntry = new UIEntryType();

            return View(blankEntry);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UIEntryType newEntry)
        {
            if (ModelState.IsValid)
            {
                var entry = Mapper.Map<StorageModel.Entry>(newEntry);
                await EntryRepository.SaveEntry(entry);

                return RedirectToAction("Index", "Home");
            }

            return View(newEntry);
        }
	}
}
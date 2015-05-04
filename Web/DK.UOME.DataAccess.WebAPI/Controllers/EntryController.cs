using DK.Framework.Web;
using DK.Framework.Web.WebAPI;
using DK.UOME.DataAccess.DataModel;
using DK.UOME.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DK.UOME.DataAccess.WebAPI.Controllers
{
    public class EntryController : WebAPIBaseController
    {
        [Import]
        public IEntryDataAccess EntryDataAccess { get; set; }
        
        public async Task<IList<Entry>> GetAllEntries()
        {
            return await EntryDataAccess.LoadEntries();
        }

        public async Task<Entry> GetEntry(int id)
        {
            var entries = await EntryDataAccess.LoadEntries();
            var entry = entries.Where(item => item.Id == id).FirstOrDefault();

            if (entry != null)
            {
                return entry;
            }

            return null;
        }

        public HttpResponseMessage PostEntry(Entry item)
        {
            EntryDataAccess.AddEntry(item);

            var response = Request.CreateResponse<Entry>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public async Task PutEntry(int id, Entry dirtyEntry)
        {
            dirtyEntry.Id = id;

            var entries = await EntryDataAccess.LoadEntries();
            var existingEntry = entries.Where(item => item.Id == id).FirstOrDefault();

            if (null != existingEntry)
            {
                existingEntry.DueDate = dirtyEntry.DueDate;
                existingEntry.Note = dirtyEntry.Note;
                existingEntry.OtherParty = dirtyEntry.OtherParty;
                existingEntry.Thing = dirtyEntry.Thing;
            }
        }

        public async Task DeleteEntry(int id)
        {
            if (await EntryDataAccess.EntryExists(id))
            {
                await EntryDataAccess.DeleteEntry(id);
            }
        }
    }
}

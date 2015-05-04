using AutoMapper;
using System;
using System.Diagnostics;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Web.UI.DataModel;

namespace DK.UOME.Web.UI.MappingConfigurations.ValueResolvers
{
    /// <summary>
    /// Resolves entry type values from storage model to UI model.
    /// </summary>
    public class StorageToUIEntryTypeValueResolver : ValueResolver<StorageModel.Entry, UIModel.EntryType>
    {
        protected override UIModel.EntryType ResolveCore(StorageModel.Entry source)
        {
            switch (source.Type)
            {
                case 0:
                    return UIModel.EntryType.Loaned;

                case 1:
                    return UIModel.EntryType.Borrowed;

                default:
                    Debug.Fail(string.Format("Entry type '{0}' is not supported.", source.Type));
                    throw new Exception(string.Format("Entry type '{0}' is not supported.", source.Type));
            }
        }
    }
}
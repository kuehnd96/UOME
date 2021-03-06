﻿using AutoMapper;
using System;
using System.Diagnostics;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Store.UI.DataModel.UWP;

namespace DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.ValueResolvers
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
                    Debug.Assert(false, string.Format("Entry type '{0}' is not supported.", source.Type));
                    throw new ArgumentException(string.Format("Entry type '{0}' is not supported.", source.Type));
            }
        }
    }
}
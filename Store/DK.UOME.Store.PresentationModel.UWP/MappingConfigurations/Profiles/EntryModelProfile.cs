using AutoMapper;
using DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.ValueResolvers;
using System;
using System.Collections.Generic;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Store.UI.DataModel.UWP;

namespace DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.Profiles
{
    internal class EntryModelProfile  : Profile
    {
        protected override void Configure()
        {           
            // Borrowed entries
            Mapper.CreateMap<StorageModel.Entry, UIModel.BorrowedEntry>()
                    .ForMember(target => target.Type, opt => opt.ResolveUsing<StorageToUIEntryTypeValueResolver>());

            Mapper.CreateMap<UIModel.BorrowedEntry, StorageModel.Entry>()
                .ForMember(target => target.Type, opt => opt.ResolveUsing(src =>
                {
                    return (int)src.Type;
                }));


            // Loanded entries
            Mapper.CreateMap<StorageModel.Entry, UIModel.LoanedEntry>()
                    .ForMember(target => target.Type, opt => opt.ResolveUsing<StorageToUIEntryTypeValueResolver>());

            Mapper.CreateMap<UIModel.LoanedEntry, StorageModel.Entry>()
                .ForMember(target => target.Type, opt => opt.ResolveUsing(src =>
                {
                    return (int)src.Type;
                }));
        }

        /// <summary>
        /// Dynamically maps a storage entry to its corresponding UI entry sub-class.
        /// </summary>
        /// <param name="sourceEntry">The source entry to map. Cannot be null.</param>
        /// <returns>The mapped entry in its abstract form.</returns>
        public static UIModel.Entry MapEntry(StorageModel.Entry sourceEntry)
        {
            if (sourceEntry == null)
            {
                throw new ArgumentNullException("sourceEntry");
            }

            switch (sourceEntry.Type)
            {
                case 0:
                    return Mapper.Map<UIModel.LoanedEntry>(sourceEntry);

                case 1:
                    return Mapper.Map<UIModel.BorrowedEntry>(sourceEntry);
                
                default:
                    throw new Exception(string.Format("No type exists for entry type {0}", sourceEntry.Type));
            }
        }

        /// <summary>
        /// Dynamically maps a collection of storage entries to their corresponding UI entry sub-classes.
        /// </summary>
        /// <param name="sourceEntries">The collection of entries to map. Cannot be null.</param>
        /// <returns>The mapped entries in their abstract form.</returns>
        public static List<UIModel.Entry> MapEntryList(IList<StorageModel.Entry> sourceEntries)
        {
            if (sourceEntries == null)
            {
                throw new ArgumentNullException("sourceEntries");
            }

            List<UIModel.Entry> entries = new List<UIModel.Entry>(sourceEntries.Count);

            foreach (var sourceEntry in sourceEntries)
            {
                entries.Add(MapEntry(sourceEntry));
            }

            return entries;
        }
    }
}
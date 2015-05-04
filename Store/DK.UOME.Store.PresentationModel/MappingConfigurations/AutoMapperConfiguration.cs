using AutoMapper;
using DK.UOME.Store.PresentationModel.MappingConfigurations.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DK.UOME.Store.PresentationModel.MappingConfigurations
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Configuration.AddProfile(new EntryModelProfile());
        }
    }
}
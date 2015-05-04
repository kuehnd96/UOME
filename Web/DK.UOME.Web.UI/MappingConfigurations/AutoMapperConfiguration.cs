using AutoMapper;
using DK.UOME.Web.UI.MappingConfigurations.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DK.UOME.Web.UI.MappingConfigurations
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Configuration.AddProfile(new EntryModelProfile());
        }
    }
}
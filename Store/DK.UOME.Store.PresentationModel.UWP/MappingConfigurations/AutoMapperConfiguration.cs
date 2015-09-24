using AutoMapper;
using DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.Profiles;

namespace DK.UOME.Store.PresentationModel.UWP.MappingConfigurations
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Configuration.AddProfile(new EntryModelProfile());
        }
    }
}
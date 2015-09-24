using AutoMapper;
using UIModel = DK.UOME.Store.UI.DataModel.UWP;

namespace DK.UOME.Store.PresentationModel.UWP.MappingConfigurations.ValueResolvers
{
    /// <summary>
    /// Resolves entry type values from UI model to storage model.
    /// </summary>
    public class UIToStorageEntryTypeValueResolver : ValueResolver<UIModel.Entry, int>
    {
        protected override int ResolveCore(UIModel.Entry source)
        {
            return (int)source.Type;
        }
    }
}
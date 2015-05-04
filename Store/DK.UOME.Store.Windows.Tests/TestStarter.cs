using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.PresentationModel.MappingConfigurations;
using DK.UOME.Store.PresentationModel.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Reflection;

namespace DK.UOME.Store.Windows.Tests
{
    /// <summary>
    /// Houses the startup functionality for this test assembly.
    /// </summary>
    [TestClass]
    public sealed class TestStarter
    {
        [AssemblyInitialize]
        public static void Startup(TestContext context)
        {
            CompositionStarter starter = new CompositionStarter(null, null);

            // Repositories are left out so the fake one is used
            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(TestStarter).GetTypeInfo().Assembly,
                typeof(DK.UOME.DataAccess.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.UI.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.DataAccess.Local.EntryDataAccess).GetTypeInfo().Assembly,
                typeof(MainViewModel).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);

            AutoMapperConfiguration.Configure();
        }
    }
}

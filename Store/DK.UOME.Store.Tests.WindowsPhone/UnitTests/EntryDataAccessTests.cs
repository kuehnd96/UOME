using DK.Framework.Store;
using DK.Framework.Store.WinPhone8.Controls;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.Repositories;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DK.UOME.Store.Tests.WindowsPhone.UnitTests
{
    [TestClass]
    public sealed class EntryDataAccessTests
    {
        [AssemblyInitialize]
        public void Startup()
        {
            CompositionStarter starter = new CompositionStarter(null, null);

            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(EntryDataAccessTests).GetTypeInfo().Assembly,
                typeof(DK.UOME.DataAccess.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(EntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.UI.DataModel.Entry).GetTypeInfo().Assembly,
                typeof(ValidationMessageControl).GetTypeInfo().Assembly,
                typeof(DK.UOME.Store.WindowsPhone.DataAccess.Local.EntryDataAccess).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);
        }

        //[TestMethod]
        public void WP8GetAllEntriesTest()
        {
            EntryRepository entryRepository = new EntryRepository();
        }

        // Test Save new
        // Test save update
        // Test delete single
        // Test delete multiple

        // All the while testing update actions and exists
    }
}

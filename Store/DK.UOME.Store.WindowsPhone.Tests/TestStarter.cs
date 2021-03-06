﻿using DK.Framework.Store;
using DK.UOME.DataAccess.Interfaces;
using DK.UOME.Repositories.Interfaces;
using DK.UOME.Store.PresentationModel.ViewModels;
using DK.UOME.Store.Repositories;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Reflection;

namespace DK.UOME.Store.WindowsPhone.Tests
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

            List<Assembly> compositionAssemblies = new List<Assembly>() { 
                typeof(TestStarter).GetTypeInfo().Assembly,
                typeof(DK.UOME.DataAccess.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(NavigationService).GetTypeInfo().Assembly,
                typeof(IEntryRepository).GetTypeInfo().Assembly, 
                typeof(EntryRepository).GetTypeInfo().Assembly, 
                typeof(IEntryDataAccess).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.UI.DataModel.Entry).GetTypeInfo().Assembly, 
                typeof(DK.UOME.Store.DataAccess.Local.EntryDataAccess).GetTypeInfo().Assembly,
                typeof(MainViewModel).GetTypeInfo().Assembly};

            starter.Configure(compositionAssemblies, null);
        }
    }
}

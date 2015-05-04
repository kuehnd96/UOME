using DK.Framework.Web;
using DK.UOME.DataAccess.Local;
using DK.UOME.Web.Repositories;
using DK.UOME.Web.UI.MappingConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StorageModel = DK.UOME.DataAccess.DataModel;
using UIModel = DK.UOME.Web.UI.DataModel;
using DK.Framework.Web.MVC;

namespace DK.UOME.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperConfiguration.Configure();

            StartComposition();

            ControllerBuilder.Current.SetControllerFactory(new MEFControllerFactory());
        }

        void StartComposition()
        {
            CompositionStarter compositionStarter = new CompositionStarter(this, null);

            compositionStarter.Configure(Assembly.GetExecutingAssembly(),
                typeof(StorageModel.Entry).GetTypeInfo().Assembly,
                typeof(UIModel.Entry).GetTypeInfo().Assembly,
                typeof(EntryRepository).GetTypeInfo().Assembly,
                typeof(CompositionStarter).GetTypeInfo().Assembly,
                typeof(EntryDataAccess).GetTypeInfo().Assembly);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Reflection;
using DK.Framework.Web;
using DK.UOME.DataAccess.DataModel;
using DK.UOME.DataAccess.Local;

namespace DK.UOME.DataAccess.WebAPI
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            StartComposition();
        }

        void StartComposition()
        {
            CompositionStarter compositionStarter = new CompositionStarter(this, null);

            compositionStarter.Configure(Assembly.GetExecutingAssembly(),
                typeof(Entry).GetTypeInfo().Assembly,
                typeof(EntryDataAccess).GetTypeInfo().Assembly,
                typeof(CompositionStarter).GetTypeInfo().Assembly);
                
        }
    }
}
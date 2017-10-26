using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lexfy.Web.Interface
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DependencyResolver.SetResolver(((IServiceProvider)SimpleInjectorContainer.RegisterServices()).GetService,
                type => (IEnumerable<object>)((IServiceProvider)SimpleInjectorContainer.RegisterServices()).GetService(typeof(IEnumerable<>).MakeGenericType(type))
                ?? Enumerable.Empty<object>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}

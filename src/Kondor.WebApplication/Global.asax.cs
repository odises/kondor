using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Kondor.Data;

namespace Kondor.WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof (MvcApplication).Assembly);

            builder.RegisterType<KondorDataContext>().As<IDbContext>();


            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
        }
    }
}

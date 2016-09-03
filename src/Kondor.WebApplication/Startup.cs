using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kondor.WebApplication.Startup))]
namespace Kondor.WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

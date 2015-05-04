using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DK.UOME.Web.UI.Startup))]
namespace DK.UOME.Web.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

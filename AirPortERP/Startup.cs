using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AirPortERP.Startup))]
namespace AirPortERP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QREST.Startup))]
namespace QREST
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
using Microsoft.Owin;
using Owin;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;

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
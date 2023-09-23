using API.Middleware;
using API.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Host.SystemWeb;

[assembly: OwinStartup(typeof(API.Startup))]
namespace API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            app.Use<CustomAuthenticationMiddleware>();
            ConfigureAuth(app);
            app.UseOAuthAuthorizationServer(OAuthOptions);

        }
    }
}
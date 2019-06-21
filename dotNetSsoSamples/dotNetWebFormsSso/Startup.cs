using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(dotNetWebFormsSso.Startup))]

namespace dotNetWebFormsSso
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ApplicationConfig.Register();
            AuthenticationConfig.Register(app);
        }
    }
}
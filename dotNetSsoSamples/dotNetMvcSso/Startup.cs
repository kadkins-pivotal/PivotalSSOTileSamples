using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(dotNetMvcSso.Startup))]

namespace dotNetMvcSso
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthenticationConfig.Register(app);
        }
    }
}

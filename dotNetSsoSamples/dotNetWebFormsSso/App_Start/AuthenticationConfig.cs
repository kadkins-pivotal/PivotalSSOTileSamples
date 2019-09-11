using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Security.Claims;
using Steeltoe.Security.Authentication.CloudFoundry.Owin;
using System.Web.Helpers;
using Steeltoe.Security.Authentication.CloudFoundry;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;

namespace dotNetWebFormsSso
{
    public class AuthenticationConfig
    {
        public static void Register(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(
                DefaultAuthenticationTypes.ExternalCookie);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                LoginPath = new PathString("/"),
//                CookieSecure = CookieSecureOption.Always,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            app.UseCloudFoundryOpenIdConnect(
                ApplicationConfig.Configuration,
                CloudFoundryDefaults.AuthenticationScheme,
                ApplicationConfig.LoggerFactory);

            app.Use(async (context, next) =>
            {
                await next.Invoke();

                if (String.IsNullOrEmpty(context.Authentication.User.Identity.Name))
                {
                    context.Authentication.Challenge(
                        CloudFoundryDefaults.AuthenticationScheme);
                }
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
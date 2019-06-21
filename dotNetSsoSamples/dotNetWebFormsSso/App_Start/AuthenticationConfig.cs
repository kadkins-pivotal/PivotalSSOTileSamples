using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Security.Claims;
using Steeltoe.Security.Authentication.CloudFoundry.Owin;
using System.Web.Helpers;
using Steeltoe.Security.Authentication.CloudFoundry;
using Microsoft.AspNet.Identity;

namespace dotNetWebFormsSso
{
    public class AuthenticationConfig
    {
        public static void Register(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                if (context.Request.Headers["X-Forwarded-Proto"] == "https")
                {
                    context.Request.Scheme = "https";
                }
                return next();
            });

            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                CookieSecure = CookieSecureOption.Always,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(5)
            });

            app.UseCloudFoundryOpenIdConnect(
                ApplicationConfig.Configuration,
                CloudFoundryDefaults.AuthenticationScheme,
                ApplicationConfig.LoggerFactory);

            app.Use(async (Context, next) =>
            {
                await next.Invoke();

                if (String.IsNullOrEmpty(Context.Authentication.User.Identity.Name))
                {
                    Context.Authentication.Challenge(CloudFoundryDefaults.AuthenticationScheme);
                }
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
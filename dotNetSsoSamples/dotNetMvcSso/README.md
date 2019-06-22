# Pivotal Application Service - .Net SSO

This example was created with .Net Framework 4.5+, SteelToe 2.2.0.0, PCF PAS 2.5, PCF SSO Tile 1.9.

## .Net Framework MVC SSO Sample

This is a guide to integrate a .Net framework MVC web application with the Pivotal SSO Tile. This project is an example of a completed integration.

The sample provides authentication to all entry points of an application. It is meant to provide authentication as IIS would when Windows authentication is enabled.

### Integrating Your Application

#### Install the Nuget Package References

- Install **Microsoft.Owin.Security.Cookies**
- Install **Microsoft.Owin.Host.SystemWeb**
- Install **Microsoft.AspNet.WebHelpers**
- Install **Microsoft.AspNet.Identity.Owin**
- Install **Microsoft.Extensions.Logging.Console**
- Install **Steeltoe.Security.Authentication.CloudFoundryOwin**

#### Add the Code

##### Add the App_Start Folder

By convention the startup classes are stored in folder called **App_Start**. If you don't have one go ahead and create it in the root folder or your project.

##### Add the ApplicationConfig Class

Add a class called **ApplicationConfig** to the **App_Start** folder. Replace the contents with the following code and update the namespace to match your application namespace. 

```C#
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.IO;

namespace dotNetMvcSso
{
    public class ApplicationConfig
    {
        public static IConfiguration Configuration { get; private set; }
        public static LoggerFactory LoggerFactory { get; private set; }

        static ApplicationConfig()
        {
            Register();
        }

        public static void Register()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(GetContentRoot())
                .AddEnvironmentVariables()
                .AddCloudFoundry();

            Configuration = builder.Build();

            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddConsole(LogLevel.Debug);
        }

        private static string GetContentRoot()
        {
            var basePath =
               (string)AppDomain.CurrentDomain.GetData(AppContext.BaseDirectory) ??
               AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(basePath);
        }
    }
}
```

##### Add the AuthenticationConfig Class

Add a class called **AuthenticationConfig** to the **App_Start** folder. Replace the contents with the following code and update the namespace to match your application namespace. 

```C#
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Security.Claims;
using Steeltoe.Security.Authentication.CloudFoundry.Owin;
using System.Web.Helpers;
using Steeltoe.Security.Authentication.CloudFoundry;
using Microsoft.AspNet.Identity;

namespace dotNetMvcSso
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
                else
                {
                    context.Response.StatusCode = 302;
                    context.Response.Redirect(
                        $"https://{context.Request.Host}{context.Request.Path}"
                        + $"{context.Request.QueryString}");
                }
                return next();
            });

            app.SetDefaultSignInAsAuthenticationType(
                DefaultAuthenticationTypes.ExternalCookie);

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
```

##### Add the Owin Startup Class

In Visual Studio, add the Owin startup class. Access the Add New Item menu by typing Ctrl+Shift+A. Search for "owin", select it, change the name to **Startup**, and click Add.
Replace the contents with the following code and update the namespace to match your application namespace. 

Note: If you already have an Owin Startup class, add a call to AuthenticationConfig.Register(). This call should be at the beginning of your Owin pipeline.

```C#
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
```

##### Add the manifest

Create a new file in your project called **manifest.yml**. Copy the contents below into the file and change the application name and the name of the SSO Service binding to match your environment. In the sample you will change **dotNetMvcSso** and **PingFederate Sandbox** accordingly. Change the properties of the file so that the property **Copy to Output Directory** is equaly to **Copy Always**. 

```yaml
applications:
- name: dotNetMvcSso
  buildpacks:
  - hwc_buildpack
  disk_quota: 1G
  instances: 1
  memory: 1G
  services:
  - PingFederate Sandbox
  stack: windows
```

### Push your application

You are now ready to publish and push your application!

### References

[Deploying .NET Apps](https://docs.pivotal.io/pivotalcf/2-5/windows/develop.html)

[Pivotal Single Sign-On Tile](https://docs.pivotal.io/p-identity/1-9/index.html)

[Steeltoe Security](http://steeltoe.io/docs/steeltoe-security/)


# .Net Framework Web Forms SSO Sample

This is a guide to integrate a web forms application with the Pivotal SSO Tile. The sample application is an example of an finished integration.

## Running the Sample Locally

TODO

## Running the Sample on PCF

TODO

## Integrating Your Application

### Install the Nuget Package References

- Install **Microsoft.Owin.Security.Cookies**
- Install **Microsoft.Extensions.Logging.Console**
- Install **Steeltoe.Security.Authentication.CloudFoundryOwin**
- Install **Microsoft.AspNet.WebHelpers**
- Install **Microsoft.AspNet.Identity.Owin**
- Install **Microsoft.Owin.Host.SystemWeb**

### Add the Code

#### Add the Owin Startup Class

In Visual Studio, add the Owin startup class. Access the Add New Item menu by typing Ctrl+Shift+A. Search for "owin", select it, change the name to "Startup.cs", and click Add.

#### Add the App_Start Folder

By convention the startup classes are stored in folder called "App_Start". If you don't have one go ahead and create it.

#### Add the ApplicationConfig Class

Add a class called ApplicationConfig in the App_Start folder. Add the following code and update the namespace according to your application namespace. 

```C#
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.IO;

namespace dotNetWebFormsSso
{
    public class ApplicationConfig
    {
        public static IConfiguration Configuration { get; private set; }
        public static LoggerFactory LoggerFactory { get; private set; }

        public static void RegisterConfig()
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(GetContentRoot())
                .AddEnvironmentVariables()
                .AddCloudFoundry();

            Configuration = builder.Build();

            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddConsole(LogLevel.Debug);
        }

        public static string GetContentRoot()
        {
            var basePath =
               (string)AppDomain.CurrentDomain.GetData(AppContext.BaseDirectory) ??
               AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(basePath);
        }
    }
}
```












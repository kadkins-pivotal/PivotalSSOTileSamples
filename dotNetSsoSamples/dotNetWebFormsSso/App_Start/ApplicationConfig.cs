using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System.IO;
using System.Collections.Generic;

namespace dotNetWebFormsSso
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
                //uncomment if using self signed certificates
                //.AddInMemoryCollection(new Dictionary<string, string>() { { "security:oauth2:client:validateCertificates", "False" } })
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
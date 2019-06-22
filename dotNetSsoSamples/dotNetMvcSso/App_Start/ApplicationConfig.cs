﻿using System;
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
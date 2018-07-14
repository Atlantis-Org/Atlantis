using System;
using System.IO;
using System.Reflection;
using Followme.AspNet.Core.FastCommon.Components;
using Microsoft.Extensions.Configuration;

namespace Followme.AspNet.Core.FastCommon.Configurations
{
    public static class AspNetCoreConfigurationExtension
    {
        private static IConfigurationRoot _aspNetConfiguration;
        private static string _rootPath;

        public static Configuration InitalizeNetCore(this Configuration configuration)
        {
            _aspNetConfiguration=new ConfigurationBuilder().SetBasePath(_rootPath)
                                    .AddJsonFile("appsetting.json")
                                    .Build();

            return configuration;
        }

        public static Configuration InitalizeNetCoreWithEnvironment(this Configuration configuration)
        {
            _aspNetConfiguration = new ConfigurationBuilder().SetBasePath(_rootPath)
                .AddJsonFile($"configs/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
                .Build();

            return configuration;
        }

        public static Configuration SetRootPath<TStart>(this Configuration configuration)
        {
            _rootPath=Path.GetDirectoryName( typeof(TStart).Assembly.Location);
            return configuration;
        }
        
        public static T GetSetting<T>(this Configuration configuration,string name)
        {
            return _aspNetConfiguration.GetSection(name).Get<T>();
        }

        public static IConfiguration GetAspNetCoreConfiguration(this Configuration configuration)
        {
            return _aspNetConfiguration;
        }

        public static string GetRootPath(this Configuration configuration)
        {
            return _rootPath;
        }
    }

    public class Useless
    {

    }
}
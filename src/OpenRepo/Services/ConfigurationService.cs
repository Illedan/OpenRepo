using System;
using System.Diagnostics;
using System.IO;

namespace OpenRepo.Services
{
    public static class ConfigurationService
    {
        private static readonly string DefaultConfig =
            @"
# Welcome to OpenRepo
# Created by https://github.com/Illedan
# Below you can view a sample configuration if you remove the #s.
#Local:
#    c:myPath/
#Personal:
#    nuget https://www.nuget.org".Replace("\n", Environment.NewLine);

        private const string OldConfigurationFileName = "OpenRepoConfiguration.yaml";
        private const string ConfigurationFileName = "OpenRepoConfiguration.txt";

        private static string OldConfigurationPath => Path.Combine(StoragelocationService.StorageDirectory, OldConfigurationFileName);
        private static string ConfigurationPath => Path.Combine(StoragelocationService.StorageDirectory, ConfigurationFileName);

        public static string GetConfig()
        {
            if (File.Exists(ConfigurationPath))
            {
                return File.ReadAllText(ConfigurationPath);
            }

            if (File.Exists(OldConfigurationPath))
            {
                var config = File.ReadAllText(OldConfigurationPath);
                File.WriteAllText(ConfigurationPath, config);
                return config;
            }

            File.WriteAllText(ConfigurationPath, DefaultConfig);
            EditConfig();
            return DefaultConfig;
        }

        public static void EditConfig()
        {
            StartProgramService.StartProgram(ConfigurationPath);
        }
    }
}

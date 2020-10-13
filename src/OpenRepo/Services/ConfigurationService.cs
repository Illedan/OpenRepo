using System.Diagnostics;
using System.IO;

namespace OpenRepo.Services
{
    public static class ConfigurationService
    {
        private const string DefaultConfig = "# Welcome to OpenRepo\n# Created by https://github.com/Illedan \n# Below you can view a sample configuration if you remove the #s. \n#Local:\n#    c:myPath/\n\n#Personal:\n#    nuget https://www.nuget.org";

        private const string ConfigLocation = "OpenRepoConfiguration.yaml";
        private const string NewConfigFile = "OpenRepoConfiguration.txt";

        private static string OldConfigurationPath => Path.Combine(StoragelocationService.StorageDirectory, ConfigLocation);
        private static string ConfigurationPath => Path.Combine(StoragelocationService.StorageDirectory, NewConfigFile);

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
            Process.Start(ConfigurationPath);
            return DefaultConfig;
        }

        public static void EditConfig()
        {
            StartProgramService.StartProgram(ConfigurationPath);
        }
    }
}

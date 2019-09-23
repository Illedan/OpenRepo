using System.IO;

namespace OpenRepo.Services
{
    public static class ConfigurationService
    {
        private const string ConfigLocation = "OpenRepoConfiguration.yaml";

        public static string ConfigurationPath => Path.Combine(StoragelocationService.StorageDirectory, ConfigLocation);
    }
}

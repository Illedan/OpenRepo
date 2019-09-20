using System.IO;

namespace OpenRepo.Services
{
    public static class ConfigurationService
    {
        private const string ConfigLocation = "Configuration.yaml";

        public static string ConfigurationPath => Path.Combine(AssemblyLocationService.AssemblyDirectory, ConfigLocation);
    }
}

using System;
using OpenRepo.Contracts;

namespace Illedan.OpenRepo.Providers.Settings
{
    public class SettingsProviderFactory : IProviderFactory
    {
        public string Id => "Settings";

        public IProvider GetProvider(string configuration)
        {
            return new SettingsProvider(configuration);
        }
    }
}

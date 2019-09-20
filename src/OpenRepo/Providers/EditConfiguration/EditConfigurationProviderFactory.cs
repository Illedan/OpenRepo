using OpenRepo.Contracts;

namespace OpenRepo.Providers.EditConfiguration
{
    public class EditConfigurationProviderFactory : IProviderFactory
    {
        public string Id => "EditConfig";

        public IProvider GetProvider(string configuration)
        {
            return new EditConfigurationProvider();
        }
    }
}

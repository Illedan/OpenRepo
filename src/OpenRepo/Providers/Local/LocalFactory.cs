using OpenRepo.Contracts;

namespace OpenRepo.Providers.Local
{
    public class LocalFactory : IProviderFactory
    {
        public string Id => "Local";

        public IProvider GetProvider(string configuration)
        {
            return new LocalProvider(configuration.Trim());
        }
    }
}
